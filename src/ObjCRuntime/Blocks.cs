//
// Block support
//
// Copyright 2010, Novell, Inc.
// Copyright 2011 - 2013 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

// http://clang.llvm.org/docs/Block-ABI-Apple.html

namespace XamCore.ObjCRuntime {

	[StructLayout (LayoutKind.Sequential)]
#if !XAMCORE_2_0
	public 
#endif
	struct BlockDescriptor {
		public IntPtr reserved;
		public IntPtr size;
		public IntPtr copy_helper;
		public IntPtr dispose;
		public IntPtr signature;
	}

	struct XamarinBlockDescriptor {
		public BlockDescriptor descriptor;
		public int xamarin_size; // the size of the complete XamarinBlockDescriptor structure
		// followed by variable-length string (the signature)
	}

	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct BlockLiteral {
#pragma warning disable 169
#if XAMCORE_2_0
		IntPtr isa;
		BlockFlags flags;
		int reserved;
		IntPtr invoke;
		IntPtr block_descriptor;
		IntPtr local_handle;
		IntPtr global_handle;
#else
		public IntPtr isa;
		public BlockFlags flags;
		public int reserved;
		public IntPtr invoke;
		public IntPtr block_descriptor;
		public IntPtr local_handle;
		public IntPtr global_handle;
#endif
#pragma warning restore 169
#if !COREBUILD
		static IntPtr block_class = Class.GetHandle ("__NSStackBlock");

		[DllImport ("__Internal")]
		static extern IntPtr xamarin_get_block_descriptor ();

		//
		// trampoline must be static, and someone else needs to keep a ref to it
		//
		public unsafe void SetupBlock (Delegate trampoline, Delegate userDelegate)
		{
			isa = block_class;
			invoke = Marshal.GetFunctionPointerForDelegate (trampoline);
			local_handle = (IntPtr) GCHandle.Alloc (userDelegate);
			global_handle = IntPtr.Zero;
			flags = BlockFlags.BLOCK_HAS_COPY_DISPOSE | BlockFlags.BLOCK_HAS_SIGNATURE;

			/* FIXME: support stret blocks */

			// we allocate one big block of memory, the first part is the BlockDescriptor, 
			// the second part is the signature string (no need to allocate a second time
			// for the signature if we can avoid it). One descriptor is allocated for every 
			// Block; this is potentially something the static registrar can fix, since it
			// should know every possible trampoline signature.
			var signature = Runtime.ComputeSignature (trampoline.Method);
			var bytes = System.Text.Encoding.UTF8.GetBytes (signature);
			var desclen = sizeof (XamarinBlockDescriptor) + bytes.Length + 1 /* null character */;
			var descptr = Marshal.AllocHGlobal (desclen);

			block_descriptor = descptr;
			var xblock_descriptor = (XamarinBlockDescriptor *) block_descriptor;
			xblock_descriptor->descriptor = * (BlockDescriptor *) xamarin_get_block_descriptor ();
			xblock_descriptor->descriptor.signature = descptr + sizeof (BlockDescriptor) + 4 /* signature_length */;
			xblock_descriptor->xamarin_size = desclen;
			Marshal.Copy (bytes, 0, xblock_descriptor->descriptor.signature, bytes.Length);
			Marshal.WriteByte (xblock_descriptor->descriptor.signature + bytes.Length, 0); // null terminate string
		}

		public void CleanupBlock ()
		{
			GCHandle.FromIntPtr (local_handle).Free ();
			Marshal.FreeHGlobal (block_descriptor);
		}

		public object Target {
			get {
				if (global_handle != IntPtr.Zero)
					return GCHandle.FromIntPtr (global_handle).Target;
				return GCHandle.FromIntPtr (local_handle).Target;
			}
		}

		public T GetDelegateForBlock<T> () where T: class
		{
			return (T) (object) Runtime.GetDelegateForBlock (invoke, typeof (T));
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public static bool IsManagedBlock (IntPtr block)
		{
			if (block == IntPtr.Zero)
				throw new ArgumentNullException ("block");

			BlockLiteral* literal = (BlockLiteral *) block;
			BlockDescriptor* descriptor = (BlockDescriptor *) xamarin_get_block_descriptor ();
			return descriptor->copy_helper == ((BlockDescriptor *) literal->block_descriptor)->copy_helper;
		}

		internal static IntPtr GetBlockForDelegate (MethodInfo minfo, object @delegate)
		{
			if (@delegate == null)
				return IntPtr.Zero;

			if (!(@delegate is Delegate))
				throw ErrorHelper.CreateError (8016, "Unable to convert delegate to block for the return value for the method {0}.{1}, because the input isn't a delegate, it's a {1}. Please file a bug at http://bugzilla.xamarin.com.", minfo.DeclaringType.FullName, minfo.Name, @delegate.GetType ().FullName);
				
			var baseMethod = minfo.GetBaseDefinition ();

			var delegateProxies = baseMethod.ReturnTypeCustomAttributes.GetCustomAttributes (typeof (DelegateProxyAttribute), false);
			if (delegateProxies.Length == 0)
				throw ErrorHelper.CreateError (8011, "Unable to locate the delegate to block conversion attribute ([DelegateProxy]) for the return value for the method {0}.{1}. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name);
			
			var delegateProxy = (DelegateProxyAttribute) delegateProxies [0];
			if (delegateProxy.DelegateType == null)
				throw ErrorHelper.CreateError (8012, "Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: DelegateType is null. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name);

			var delegateProxyField = delegateProxy.DelegateType.GetField ("Handler", BindingFlags.NonPublic | BindingFlags.Static);
			if (delegateProxyField == null)
				throw ErrorHelper.CreateError (8013, "Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: DelegateType ({2}) specifies a type without a 'Handler' field. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name, delegateProxy.DelegateType.FullName);

			var handlerDelegate = delegateProxyField.GetValue (null);
			if (handlerDelegate == null)
				throw ErrorHelper.CreateError (8014, "Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: The DelegateType's ({2}) 'Handler' field is null. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name, delegateProxy.DelegateType.FullName);

			if (!(handlerDelegate is Delegate))
				throw ErrorHelper.CreateError (8015, "Invalid DelegateProxyAttribute for the return value for the method {0}.{1}: The DelegateType's ({2}) 'Handler' field is not a delegate, it's a {3}. Please file a bug at http://bugzilla.xamarin.com.", baseMethod.DeclaringType.FullName, baseMethod.Name, delegateProxy.DelegateType.FullName, handlerDelegate.GetType ().FullName);
			
			// We now have the information we need to create the block.
			// Note that we must create a heap-allocated block, so we 
			// start off by creating a stack-allocated block, and then
			// call _Block_copy, which will create a heap-allocated block
			// with the proper reference count.
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock ((Delegate) handlerDelegate, (Delegate) @delegate);
			var rv = _Block_copy (ref block);
			block.CleanupBlock ();
			return rv;
		}

		[DllImport (Messaging.LIBOBJC_DYLIB)]
		static extern IntPtr _Block_copy (ref BlockLiteral block);
#endif
	}
		
	[Flags]
#if XAMCORE_3_0
	internal
#else
	public
#endif
	enum BlockFlags : int {
		BLOCK_REFCOUNT_MASK =     (0xffff),
		BLOCK_NEEDS_FREE =        (1 << 24),
		BLOCK_HAS_COPY_DISPOSE =  (1 << 25),
		BLOCK_HAS_CTOR =          (1 << 26), /* Helpers have C++ code. */
		BLOCK_IS_GC =             (1 << 27),
		BLOCK_IS_GLOBAL =         (1 << 28),
		BLOCK_HAS_DESCRIPTOR =    (1 << 29), // This meaning was deprecated 
		BLOCK_HAS_STRET =         (1 << 29),
		BLOCK_HAS_SIGNATURE =     (1 << 30),
	}
}
