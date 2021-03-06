//
// API for the Metal framework
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2014-2015, Xamarin Inc.
//
// TODO:
//    * Provide friendly accessors instead of those IntPtrs that point to arrays
//    * MTLRenderPipelineReflection: the two arrays are NSObject
//    * Make the *array classes implement all the ICollection methods.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using System.ComponentModel;

using XamCore.CoreAnimation;
using XamCore.CoreData;
using XamCore.CoreGraphics;
using XamCore.CoreImage;
using XamCore.CoreLocation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Metal {

	public delegate void MTLDeallocator (IntPtr pointer, nuint length);

	public delegate void MTLNewComputePipelineStateWithReflectionCompletionHandler (IMTLComputePipelineState computePipelineState, MTLComputePipelineReflection reflection, NSError error);
	
	public interface IMTLCommandEncoder {}
	
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLArgument {
		[Export ("name")]
		string Name { get; }

		[Export ("type")]
		MTLArgumentType Type { get; }

		[Export ("access")]
		MTLArgumentAccess Access { get; }

		[Export ("index")]
		nuint Index { get; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }

		[Export ("bufferAlignment")]
		nuint BufferAlignment { get; }

		[Export ("bufferDataSize")]
		nuint BufferDataSize { get; }

		[Export ("bufferDataType")]
		MTLDataType BufferDataType { get; }

		[Export ("bufferStructType")]
		MTLStructType BufferStructType { get; }

		[Export ("threadgroupMemoryAlignment")]
		nuint ThreadgroupMemoryAlignment { get; }

		[Export ("threadgroupMemoryDataSize")]
		nuint ThreadgroupMemoryDataSize { get; }

		[Export ("textureType")]
		MTLTextureType TextureType { get; }

		[Export ("textureDataType")]
		MTLDataType TextureDataType { get; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLArrayType {
		[Export ("arrayLength")]
		nuint Length { get; }

		[Export ("elementType")]
		MTLDataType ElementType { get; }

		[Export ("stride")]
		nuint Stride { get; }

		[Export ("elementStructType")]
		MTLStructType ElementStructType ();

		[Export ("elementArrayType")]
		MTLArrayType ElementArrayType ();
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLCommandEncoder {
		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("endEncoding")]
		void EndEncoding ();

		[Abstract, Export ("insertDebugSignpost:")]
		void InsertDebugSignpost (string signpost);

		[Abstract, Export ("pushDebugGroup:")]
		void PushDebugGroup (string debugGroup);

		[Abstract, Export ("popDebugGroup")]
		void PopDebugGroup ();
	}

	public interface IMTLBuffer {}
	
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLBuffer : MTLResource {
		[Abstract, Export ("length")]
		nuint Length { get; }

		[Abstract, Export ("contents")]
		IntPtr Contents { get; }
#if MONOMAC
		[Abstract, Export ("didModifyRange:")]
		void DidModify (NSRange range);
#else
		[Abstract, Export ("newTextureWithDescriptor:offset:bytesPerRow:")]
		IMTLTexture CreateTexture (MTLTextureDescriptor descriptor, nuint offset, nuint bytesPerRow);
#endif
	}

	public interface IMTLCommandBuffer {}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLCommandBuffer {

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("commandQueue")]
		IMTLCommandQueue CommandQueue { get; }

		[Abstract, Export ("retainedReferences")]
		bool RetainedReferences { get; }

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("status")]
		MTLCommandBufferStatus Status { get; }

		[Abstract, Export ("error")]
		NSError Error { get; }

		[Abstract, Export ("enqueue")]
		void Enqueue ();

		[Abstract, Export ("commit")]
		void Commit ();

		[Abstract, Export ("addScheduledHandler:")]
		void AddScheduledHandler (Action<IMTLCommandBuffer> block);

		[Abstract, Export ("waitUntilScheduled")]
		void WaitUntilScheduled ();

		[Abstract, Export ("addCompletedHandler:")]
		void AddCompletedHandler (Action<IMTLCommandBuffer> block);

		[Abstract, Export ("waitUntilCompleted")]
		void WaitUntilCompleted ();

		[Abstract, Export ("blitCommandEncoder")]
		IMTLBlitCommandEncoder BlitCommandEncoder { get; }

		[Abstract, Export ("computeCommandEncoder")]
		IMTLComputeCommandEncoder ComputeCommandEncoder { get; }

		[Field ("MTLCommandBufferErrorDomain")]
		NSString ErrorDomain { get; }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("parallelRenderCommandEncoderWithDescriptor:")]
		IMTLParallelRenderCommandEncoder CreateParallelRenderCommandEncoder (MTLRenderPassDescriptor renderPassDescriptor);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("presentDrawable:")]
		void PresentDrawable (IMTLDrawable drawable);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("presentDrawable:atTime:")]
		void PresentDrawable (IMTLDrawable drawable, double presentationTime);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("renderCommandEncoderWithDescriptor:")]
		IMTLRenderCommandEncoder CreateRenderCommandEncoder (MTLRenderPassDescriptor renderPassDescriptor);
	}

	public interface IMTLCommandQueue {}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLCommandQueue {

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("commandBuffer")]
		[Autorelease]
		IMTLCommandBuffer CommandBuffer ();

		[Abstract, Export ("commandBufferWithUnretainedReferences")]
		[Autorelease]
		IMTLCommandBuffer CommandBufferWithUnretainedReferences ();

		[Abstract, Export ("insertDebugCaptureBoundary")]
		void InsertDebugCaptureBoundary ();
	}

	public interface IMTLComputeCommandEncoder {}
	
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLComputeCommandEncoder : MTLCommandEncoder {
		[Abstract, Export ("setComputePipelineState:")]
		void SetComputePipelineState (IMTLComputePipelineState state);

		[Abstract, Export ("setBuffer:offset:atIndex:")]
		void SetBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[Abstract, Export ("setTexture:atIndex:")]
		void SetTexture (IMTLTexture texture, nuint index);

		[Abstract, Export ("setSamplerState:atIndex:")]
		void SetSamplerState (IMTLSamplerState sampler, nuint index);

		[Abstract, Export ("setSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetSamplerState (IMTLSamplerState sampler, float /* float, not CGFloat */ lodMinClamp, float /* float, not CGFloat */ lodMaxClamp, nuint index);

		[Abstract, Export ("setThreadgroupMemoryLength:atIndex:")]
		void SetThreadgroupMemoryLength (nuint length, nuint index);

		[Abstract, Export ("dispatchThreadgroups:threadsPerThreadgroup:")]
#if XAMCORE_2_0
		void DispatchThreadgroups (MTLSize threadgroupsPerGrid, MTLSize threadsPerThreadgroup);
#else
		void SispatchThreadgroups (MTLSize threadgroupsPerGrid, MTLSize threadsPerThreadgroup);
#endif

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)]
		[Export ("dispatchThreadgroupsWithIndirectBuffer:indirectBufferOffset:threadsPerThreadgroup:")]
		void DispatchThreadgroups (IMTLBuffer indirectBuffer, nuint indirectBufferOffset, MTLSize threadsPerThreadgroup);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("setBuffers:offsets:withRange:")]
		void SetBuffers (IMTLBuffer [] buffers, IntPtr offsets, NSRange range);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("setSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetSamplerStates (IMTLSamplerState [] samplers, IntPtr floatArrayPtrLodMinClamps, IntPtr floatArrayPtrLodMaxClamps, NSRange range);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("setSamplerStates:withRange:")]
		void SetSamplerStates (IMTLSamplerState [] samplers, NSRange range);
		
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("setTextures:withRange:")]
		void SetTextures (IMTLTexture [] textures, NSRange range);

		[iOS (8,3)]
		[Abstract]
		[Export ("setBufferOffset:atIndex:")]
		void SetBufferOffset (nuint offset, nuint index);

		[iOS (8,3)]
		[Abstract]
		[Export ("setBytes:length:atIndex:")]
		void SetBytes (IntPtr bytes, nuint length, nuint index);
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLComputePipelineReflection {
		[Export ("arguments")]
#if XAMCORE_4_0
		MTLArgument [] Arguments { get; }
#else
		NSObject [] Arguments { get; }
#endif
	}

	public interface IMTLComputePipelineState {}
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLComputePipelineState {
		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("maxTotalThreadsPerThreadgroup")]
		nuint MaxTotalThreadsPerThreadgroup { get; }

		[Abstract, Export ("threadExecutionWidth")]
		nuint ThreadExecutionWidth { get; }
	}

	public interface IMTLBlitCommandEncoder {}
	
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLBlitCommandEncoder : MTLCommandEncoder {

#if MONOMAC
		[Abstract, Export ("synchronizeResource:")]
		void Synchronize (IMTLResource resource);

		[Abstract, Export ("synchronizeTexture:slice:level:")]
		void Synchronize (IMTLTexture texture, nuint slice, nuint level);
#endif

		[Abstract, Export ("copyFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:")]
		void CopyFromTexture (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel,  MTLOrigin sourceOrigin,  MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel,  MTLOrigin destinationOrigin);

		[Abstract, Export ("copyFromBuffer:sourceOffset:sourceBytesPerRow:sourceBytesPerImage:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:")]
		void CopyFromBuffer (IMTLBuffer sourceBuffer, nuint sourceOffset, nuint sourceBytesPerRow, nuint sourceBytesPerImage,  MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel,  MTLOrigin destinationOrigin);

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("copyFromBuffer:sourceOffset:sourceBytesPerRow:sourceBytesPerImage:sourceSize:toTexture:destinationSlice:destinationLevel:destinationOrigin:options:")]
		void CopyFromBuffer (IMTLBuffer sourceBuffer, nuint sourceOffset, nuint sourceBytesPerRow, nuint sourceBytesPerImage, MTLSize sourceSize, IMTLTexture destinationTexture, nuint destinationSlice, nuint destinationLevel, MTLOrigin destinationOrigin, MTLBlitOption options);

		[Abstract, Export ("copyFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toBuffer:destinationOffset:destinationBytesPerRow:destinationBytesPerImage:")]
		void CopyFromTexture (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, MTLOrigin sourceOrigin,  MTLSize sourceSize, IMTLBuffer destinationBuffer, nuint destinationOffset, nuint destinatinBytesPerRow, nuint destinationBytesPerImage);

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("copyFromTexture:sourceSlice:sourceLevel:sourceOrigin:sourceSize:toBuffer:destinationOffset:destinationBytesPerRow:destinationBytesPerImage:options:")]
		void CopyFromTexture (IMTLTexture sourceTexture, nuint sourceSlice, nuint sourceLevel, MTLOrigin sourceOrigin,  MTLSize sourceSize, IMTLBuffer destinationBuffer, nuint destinationOffset, nuint destinatinBytesPerRow, nuint destinationBytesPerImage, MTLBlitOption options);

		[Abstract, Export ("generateMipmapsForTexture:")]
		void GenerateMipmapsForTexture (IMTLTexture texture);

		[Abstract, Export ("fillBuffer:range:value:")]
		void FillBuffer (IMTLBuffer buffer, NSRange range, byte value);

		[Abstract, Export ("copyFromBuffer:sourceOffset:toBuffer:destinationOffset:size:")]
		void CopyFromBuffer (IMTLBuffer sourceBuffer, nuint sourceOffset, IMTLBuffer destinationBuffer, nuint destinationOffset, nuint size);
	}

	public interface IMTLDevice {}
	
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLDevice {

		[Abstract, Export ("name")]
		string Name { get; }

#if XAMCORE_4_0
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[iOS (9,0)]
		[Export ("maxThreadsPerThreadgroup")]
		MTLSize MaxThreadsPerThreadgroup { get; }

		[NoiOS]
		[NoTV]
		[Export ("lowPower")]
		bool LowPower { [Bind ("isLowPower")] get; }

		[NoiOS]
		[NoTV]
		[Export ("headless")]
		bool Headless { [Bind ("isHeadless")] get; }

		[NoiOS]
		[NoTV]
		[Export ("depth24Stencil8PixelFormatSupported")]
		bool Depth24Stencil8PixelFormatSupported { [Bind ("isDepth24Stencil8PixelFormatSupported")] get; }

		[Abstract, Export ("newCommandQueue")]
		IMTLCommandQueue CreateCommandQueue ();

		[Abstract, Export ("newCommandQueueWithMaxCommandBufferCount:")]
		IMTLCommandQueue CreateCommandQueue (nuint maxCommandBufferCount);

		[Abstract, Export ("newBufferWithLength:options:")]
		IMTLBuffer CreateBuffer (nuint length, MTLResourceOptions options);

		[Abstract, Export ("newBufferWithBytes:length:options:")]
		IMTLBuffer CreateBuffer (IntPtr pointer, nuint length, MTLResourceOptions options);

		[Abstract, Export ("newBufferWithBytesNoCopy:length:options:deallocator:")]
		IMTLBuffer CreateBufferNoCopy (IntPtr pointer, nuint length, MTLResourceOptions options, MTLDeallocator deallocator);

		[Abstract, Export ("newDepthStencilStateWithDescriptor:")]
		IMTLDepthStencilState CreateDepthStencilState (MTLDepthStencilDescriptor descriptor);

		[Abstract, Export ("newTextureWithDescriptor:")]
		IMTLTexture CreateTexture (MTLTextureDescriptor descriptor);

		[Abstract, Export ("newSamplerStateWithDescriptor:")]
		IMTLSamplerState CreateSamplerState (MTLSamplerDescriptor descriptor);

		[Abstract, Export ("newDefaultLibrary")]
		IMTLLibrary CreateDefaultLibrary ();

		[Abstract, Export ("newLibraryWithFile:error:")]
		IMTLLibrary CreateLibrary (string filepath, out NSError error);

		[Abstract, Export ("newLibraryWithData:error:")]
		IMTLLibrary CreateLibrary (NSObject data, out NSError error);

		[Abstract, Export ("newLibraryWithSource:options:error:")]
		IMTLLibrary CreateLibrary (string source, MTLCompileOptions options, out NSError error);

		[Abstract, Export ("newLibraryWithSource:options:completionHandler:")]
		void CreateLibrary (string source, MTLCompileOptions options, Action<IMTLLibrary, NSError> completionHandler);

		[Abstract, Export ("newRenderPipelineStateWithDescriptor:error:")]
		IMTLRenderPipelineState CreateRenderPipelineState (MTLRenderPipelineDescriptor descriptor, out NSError error);

		[Abstract, Export ("newRenderPipelineStateWithDescriptor:completionHandler:")]
		void CreateRenderPipelineState (MTLRenderPipelineDescriptor descriptor, Action<IMTLRenderPipelineState, NSError> completionHandler);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("newRenderPipelineStateWithDescriptor:options:reflection:error:")]
		IMTLRenderPipelineState CreateRenderPipelineState (MTLRenderPipelineDescriptor descriptor, MTLPipelineOption options, out MTLRenderPipelineReflection reflection, out NSError error);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("newRenderPipelineStateWithDescriptor:options:completionHandler:")]
		void CreateRenderPipelineState (MTLRenderPipelineDescriptor descriptor, MTLPipelineOption options, Action<IMTLRenderPipelineState, MTLRenderPipelineReflection, NSError> completionHandler);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("newComputePipelineStateWithFunction:options:reflection:error:")]
		IMTLComputePipelineState CreateComputePipelineState (IMTLFunction computeFunction, MTLPipelineOption options, out MTLComputePipelineReflection reflection, out NSError error);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("newComputePipelineStateWithFunction:completionHandler:")]
		void CreateComputePipelineState (IMTLFunction computeFunction, Action<IMTLComputePipelineState, NSError> completionHandler);

		[Abstract, Export ("newComputePipelineStateWithFunction:error:")]
		IMTLComputePipelineState CreateComputePipelineState (IMTLFunction computeFunction, out NSError error);

		[Abstract, Export ("newComputePipelineStateWithFunction:options:completionHandler:")]
		void CreateComputePipelineState (IMTLFunction computeFunction, MTLPipelineOption options, Action<IMTLComputePipelineState, MTLComputePipelineReflection, NSError> completionHandler);

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("newComputePipelineStateWithDescriptor:options:reflection:error:")]
		IMTLComputePipelineState CreateComputePipelineState (MTLComputePipelineDescriptor descriptor, MTLPipelineOption options, out MTLComputePipelineReflection reflection, out NSError error);

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("newComputePipelineStateWithDescriptor:options:completionHandler:")]
		void CreateComputePipelineState (MTLComputePipelineDescriptor descriptor, MTLPipelineOption options, MTLNewComputePipelineStateWithReflectionCompletionHandler completionHandler);

		[Abstract, Export ("supportsFeatureSet:")]
		bool SupportsFeatureSet (MTLFeatureSet featureSet);

		[iOS (9,0)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("supportsTextureSampleCount:")]
		bool SupportsTextureSampleCount (nuint sampleCount);
	}

	public interface IMTLDrawable {}
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	public partial interface MTLDrawable {
		[Abstract, Export ("present")]
		void Present ();
		
		[Abstract, Export ("presentAtTime:")]
		void Present (double presentationTime);
	}

	public interface IMTLTexture {}

	// Apple added several new *required* members in iOS 9,
	// but that breaks our binary compat, so we can't do that in our existing code.
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLTexture : MTLResource {
		[Abstract, Export ("rootResource")]
		IMTLResource RootResource { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[NullAllowed] // by default this property is null
		[Export ("parentTexture")]
		IMTLTexture ParentTexture { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("parentRelativeLevel")]
		nuint ParentRelativeLevel { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("parentRelativeSlice")]
		nuint ParentRelativeSlice { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[NullAllowed] // by default this property is null
		[Export ("buffer")]
		IMTLBuffer Buffer { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("bufferOffset")]
		nuint BufferOffset { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("bufferBytesPerRow")]
		nuint BufferBytesPerRow { get; }

		[Abstract, Export ("textureType")]
		MTLTextureType TextureType { get; }

		[Abstract, Export ("pixelFormat")]
		MTLPixelFormat PixelFormat { get; }

		[Abstract, Export ("width")]
		nuint Width { get; }

		[Abstract, Export ("height")]
		nuint Height { get; }

		[Abstract, Export ("depth")]
		nuint Depth { get; }

		[Abstract, Export ("mipmapLevelCount")]
		nuint MipmapLevelCount { get; }

		[Abstract, Export ("sampleCount")]
		nuint SampleCount { get; }

		[Abstract, Export ("arrayLength")]
		nuint ArrayLength { get; }

		[Abstract, Export ("framebufferOnly")]
		bool FramebufferOnly { [Bind ("isFramebufferOnly")] get; }

		[Abstract, Export ("newTextureViewWithPixelFormat:")]
		IMTLTexture CreateTextureView (MTLPixelFormat pixelFormat);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("usage")]
		MTLTextureUsage Usage { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("newTextureViewWithPixelFormat:textureType:levels:slices:")]
		IMTLTexture CreateTextureView (MTLPixelFormat pixelFormat, MTLTextureType textureType, NSRange levelRange, NSRange sliceRange);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("getBytes:bytesPerRow:bytesPerImage:fromRegion:mipmapLevel:slice:")]
		void GetBytes (IntPtr pixelBytes, nuint bytesPerRow, nuint bytesPerImage, MTLRegion region, nuint level, nuint slice);		

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("getBytes:bytesPerRow:fromRegion:mipmapLevel:")]
		void GetBytes (IntPtr pixelBytes, nuint bytesPerRow,  MTLRegion region, nuint level);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("replaceRegion:mipmapLevel:slice:withBytes:bytesPerRow:bytesPerImage:")]
		void ReplaceRegion (MTLRegion region, nuint level, nuint slice, IntPtr pixelBytes, nuint bytesPerRow, nuint bytesPerImage);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("replaceRegion:mipmapLevel:withBytes:bytesPerRow:")]
		void ReplaceRegion (MTLRegion region, nuint level, IntPtr pixelBytes, nuint bytesPerRow);
	}
	

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public partial interface MTLTextureDescriptor : NSCopying {

		[Export ("textureType")]
		MTLTextureType TextureType { get; set; }

		[Export ("pixelFormat")]
		MTLPixelFormat PixelFormat { get; set; }

		[Export ("width")]
		nuint Width { get; set; }

		[Export ("height")]
		nuint Height { get; set; }

		[Export ("depth")]
		nuint Depth { get; set; }

		[Export ("mipmapLevelCount")]
		nuint MipmapLevelCount { get; set; }

		[Export ("sampleCount")]
		nuint SampleCount { get; set; }

		[Export ("arrayLength")]
		nuint ArrayLength { get; set; }

		[Export ("resourceOptions")]
		MTLResourceOptions ResourceOptions { get; set; }

		[Static, Export ("texture2DDescriptorWithPixelFormat:width:height:mipmapped:")]
		MTLTextureDescriptor CreateTexture2DDescriptor (MTLPixelFormat pixelFormat, nuint width, nuint height, bool mipmapped);

		[Static, Export ("textureCubeDescriptorWithPixelFormat:size:mipmapped:")]
		MTLTextureDescriptor CreateTextureCubeDescriptor (MTLPixelFormat pixelFormat, nuint size, bool mipmapped);

		[iOS (9,0)]
		[Export ("cpuCacheMode", ArgumentSemantic.Assign)]
		MTLCpuCacheMode CpuCacheMode { get; set; }

		[iOS (9,0)]
		[Export ("storageMode", ArgumentSemantic.Assign)]
		MTLStorageMode StorageMode { get; set; }

		[iOS (9,0)]
		[Export ("usage", ArgumentSemantic.Assign)]
		MTLTextureUsage Usage { get; set; }		
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public partial interface MTLSamplerDescriptor : NSCopying {

		[Export ("minFilter")]
		MTLSamplerMinMagFilter MinFilter { get; set; }

		[Export ("magFilter")]
		MTLSamplerMinMagFilter MagFilter { get; set; }

		[Export ("mipFilter")]
		MTLSamplerMipFilter MipFilter { get; set; }

		[Export ("maxAnisotropy")]
		nuint MaxAnisotropy { get; set; }

		[Export ("sAddressMode")]
		MTLSamplerAddressMode SAddressMode { get; set; }

		[Export ("tAddressMode")]
		MTLSamplerAddressMode TAddressMode { get; set; }

		[Export ("rAddressMode")]
		MTLSamplerAddressMode RAddressMode { get; set; }

		[Export ("normalizedCoordinates")]
		bool NormalizedCoordinates { get; set; }

		[Export ("lodMinClamp")]
		float LodMinClamp { get; set; } /* float, not CGFloat */ 

		[Export ("lodMaxClamp")]
		float LodMaxClamp { get; set; } /* float, not CGFloat */

#if !MONOMAC
		[iOS (9,0)]
		[Export ("lodAverage")]
		bool LodAverage { get; set; }
#endif

		[iOS (9,0)]
		[Export ("compareFunction")]
		MTLCompareFunction CompareFunction { get; set; }

		// [NullAllowed] we can't allow setting null - even if the default value is null
		// /SourceCache/AcceleratorKit/AcceleratorKit-14.9/Framework/MTLSampler.m:240: failed assertion `label must not be nil.'
		[Export ("label")]
		string Label { get; set; }
	}

	public interface IMTLSamplerState {}
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLSamplerState  {

		[Abstract, Export ("label")]
		string Label { get; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public partial interface MTLRenderPipelineDescriptor : NSCopying {

		// [NullAllowed] we can't allow setting null - even if the default value is null
		// /SourceCache/AcceleratorKit/AcceleratorKit-14.9/Framework/MTLRenderPipeline.mm:627: failed assertion `label must not be nil.'
		[Export ("label")]
		string Label { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("vertexFunction", ArgumentSemantic.Retain)]
		IMTLFunction VertexFunction { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fragmentFunction", ArgumentSemantic.Retain)]
		IMTLFunction FragmentFunction { get; set; }

		[Export ("vertexDescriptor", ArgumentSemantic.Copy)]
		MTLVertexDescriptor VertexDescriptor { get; set; }

		[Export ("sampleCount")]
		nuint SampleCount { get; set; }

		[Export ("alphaToCoverageEnabled")]
		bool AlphaToCoverageEnabled { [Bind ("isAlphaToCoverageEnabled")] get; set; }

		[Export ("alphaToOneEnabled")]
		bool AlphaToOneEnabled { [Bind ("isAlphaToOneEnabled")] get; set; }

		[Export ("rasterizationEnabled")]
		bool RasterizationEnabled { [Bind ("isRasterizationEnabled")] get; set; }
		
		[Export ("reset")]
		void Reset ();

		[Export ("colorAttachments")]
		MTLRenderPipelineColorAttachmentDescriptorArray ColorAttachments { get; }

		[Export ("depthAttachmentPixelFormat")]
		MTLPixelFormat DepthAttachmentPixelFormat { get; set; }

		[Export ("stencilAttachmentPixelFormat")]
		MTLPixelFormat StencilAttachmentPixelFormat { get; set; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLRenderPipelineColorAttachmentDescriptorArray {

		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLRenderPipelineColorAttachmentDescriptor ObjectAtIndexedSubscript (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject (MTLRenderPipelineColorAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	public interface IMTLRenderPipelineState {}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLRenderPipelineState {

		[Abstract, Export ("label")]
		string Label { get; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLVertexBufferLayoutDescriptor : NSCopying {
		[Export ("stride", ArgumentSemantic.UnsafeUnretained)]
		nuint Stride { get; set; }

		[Export ("stepFunction", ArgumentSemantic.UnsafeUnretained)]
		MTLVertexStepFunction StepFunction { get; set; }

		[Export ("stepRate", ArgumentSemantic.UnsafeUnretained)]
		nuint StepRate { get; set; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLVertexBufferLayoutDescriptorArray {
		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLVertexBufferLayoutDescriptor ObjectAtIndexedSubscript (nuint index);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject (MTLVertexBufferLayoutDescriptor bufferDesc, nuint index);
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLVertexAttributeDescriptor : NSCopying {
		[Export ("format", ArgumentSemantic.UnsafeUnretained)]
		MTLVertexFormat Format { get; set; }

		[Export ("offset", ArgumentSemantic.UnsafeUnretained)]
		nuint Offset { get; set; }

		[Export ("bufferIndex", ArgumentSemantic.UnsafeUnretained)]
		nuint BufferIndex { get; set; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLVertexAttributeDescriptorArray {
		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLVertexAttributeDescriptor ObjectAtIndexedSubscript (nuint index);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject (MTLVertexAttributeDescriptor attributeDesc, nuint index);
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public partial interface MTLVertexDescriptor : NSCopying {
		[Static, Export ("vertexDescriptor")]
		MTLVertexDescriptor Create ();

		[Export ("reset")]
		void Reset ();

		[Export ("layouts")]
		MTLVertexBufferLayoutDescriptorArray Layouts { get; }

		[Export ("attributes")]
		MTLVertexAttributeDescriptorArray Attributes { get; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public partial interface MTLVertexAttribute {
		[Export ("attributeIndex")]
		nuint AttributeIndex { get; }

		[iOS (8,3)]
		[Export ("attributeType")]
		MTLDataType AttributeType { get; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }

		[Export ("name")]
		string Name { get; }
	}

	public interface IMTLFunction {}
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLFunction  {

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("functionType")]
		MTLFunctionType FunctionType { get; }

		[Abstract, Export ("vertexAttributes")]
		MTLVertexAttribute [] VertexAttributes { get; }

		[Abstract, Export ("name")]
		string Name { get; }
	}

	public interface IMTLLibrary {}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLLibrary  {

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("functionNames")]
		string [] FunctionNames { get; }

		[Abstract, Export ("newFunctionWithName:")]
		IMTLFunction CreateFunction (string functionName);

		[Field ("MTLLibraryErrorDomain")]
		NSString ErrorDomain { get; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public partial interface MTLCompileOptions : NSCopying {

		[NullAllowed] // by default this property is null
		[Export ("preprocessorMacros", ArgumentSemantic.Copy)]
		NSDictionary PreprocessorMacros { get; set; }

		[Export ("fastMathEnabled")]
		bool FastMathEnabled { get; set; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("languageVersion", ArgumentSemantic.Assign)]
		MTLLanguageVersion LanguageVersion { get; set; }
	}
	
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public partial interface MTLStencilDescriptor : NSCopying {
		[Export ("stencilCompareFunction")]
		MTLCompareFunction StencilCompareFunction { get; set; }

		[Export ("stencilFailureOperation")]
		MTLStencilOperation StencilFailureOperation { get; set; }

		[Export ("depthFailureOperation")]
		MTLStencilOperation DepthFailureOperation { get; set; }

		[Export ("depthStencilPassOperation")]
		MTLStencilOperation DepthStencilPassOperation { get; set; }

		[Export ("readMask")]
		uint ReadMask { get; set; } /* uint32_t */

		[Export ("writeMask")]
		uint WriteMask { get; set; } /* uint32_t */
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLStructMember {
		[Export ("name")]
		string Name { get; }

		[Export ("offset")]
		nuint Offset { get; }

		[Export ("dataType")]
		MTLDataType DataType { get; }

#if XAMCORE_4_0
		[Export ("structType")]
		MTLStructType StructType { get; }

		[Export ("arrayType")]
		MTLArrayType ArrayType { get; }
#else
		[Export ("structType")]
		MTLStructType StructType ();

		[Export ("arrayType")]
		MTLArrayType ArrayType ();
#endif
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLStructType {
		[Export ("members")]
		MTLStructMember [] Members { get; }

		[Export ("memberByName:")]
		MTLStructMember Lookup (string name);
	}

	public interface IMTLDepthStencilState {}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLDepthStencilState  {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("label")]
		string Label { get; }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("device")]
		IMTLDevice Device { get; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public partial interface MTLDepthStencilDescriptor : NSCopying {

		[Export ("depthCompareFunction")]
		MTLCompareFunction DepthCompareFunction { get; set; }

		[Export ("depthWriteEnabled")]
		bool DepthWriteEnabled { [Bind ("isDepthWriteEnabled")] get; set; }

		[Export ("frontFaceStencil", ArgumentSemantic.Copy)]
		MTLStencilDescriptor FrontFaceStencil { get; set; }

		[Export ("backFaceStencil", ArgumentSemantic.Copy)]
		MTLStencilDescriptor BackFaceStencil { get; set; }

		// [NullAllowed] we can't allow setting null - even if the default value is null
		// /SourceCache/AcceleratorKit/AcceleratorKit-14.9/Framework/MTLDepthStencil.m:393: failed assertion `label must not be nil.'
		[Export ("label")]
		string Label { get; set; }
	}

	public interface IMTLParallelRenderCommandEncoder {}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public interface MTLParallelRenderCommandEncoder : MTLCommandEncoder {
		[Abstract]
		[Export ("renderCommandEncoder")]
		[Autorelease]
		IMTLRenderCommandEncoder CreateRenderCommandEncoder ();
	}

	public interface IMTLRenderCommandEncoder {}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLRenderCommandEncoder : MTLCommandEncoder {

		[Abstract, Export ("setRenderPipelineState:")]
		void SetRenderPipelineState (IMTLRenderPipelineState pipelineState);

		[Abstract, Export ("setVertexBuffer:offset:atIndex:")]
		void SetVertexBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[Abstract, Export ("setVertexTexture:atIndex:")]
		void SetVertexTexture (IMTLTexture texture, nuint index);

		[Abstract, Export ("setVertexSamplerState:atIndex:")]
		void SetVertexSamplerState (IMTLSamplerState sampler, nuint index);

		[Abstract, Export ("setVertexSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetVertexSamplerState (IMTLSamplerState sampler, float /* float, not CGFloat */ lodMinClamp, float /* float, not CGFloat */ lodMaxClamp, nuint index);

		[Abstract, Export ("setViewport:")]
		void SetViewport (MTLViewport viewport);

		[Abstract, Export ("setFrontFacingWinding:")]
		void SetFrontFacingWinding (MTLWinding frontFacingWinding);

		[Abstract, Export ("setCullMode:")]
		void SetCullMode (MTLCullMode cullMode);

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("setDepthClipMode:")]
		void SetDepthClipMode (MTLDepthClipMode depthClipMode);

		[Abstract, Export ("setDepthBias:slopeScale:clamp:")]
		void SetDepthBias (float /* float, not CGFloat */ depthBias, float /* float, not CGFloat */ slopeScale, float /* float, not CGFloat */ clamp);

		[Abstract, Export ("setScissorRect:")]
		void SetScissorRect (MTLScissorRect rect);

		[Abstract, Export ("setTriangleFillMode:")]
		void SetTriangleFillMode (MTLTriangleFillMode fillMode);

		[Abstract, Export ("setFragmentBuffer:offset:atIndex:")]
		void SetFragmentBuffer (IMTLBuffer buffer, nuint offset, nuint index);

		[iOS (8,3)]
		[Abstract, Export ("setFragmentBufferOffset:atIndex:")]
		void SetFragmentBufferOffset (nuint offset, nuint index);

		[iOS (8,3)]
		[Abstract, Export ("setFragmentBytes:length:atIndex:")]
		void SetFragmentBytes (IntPtr bytes, nuint length, nuint index);

		[Abstract, Export ("setFragmentTexture:atIndex:")]
		void SetFragmentTexture (IMTLTexture texture, nuint index);

		[Abstract, Export ("setFragmentSamplerState:atIndex:")]
		void SetFragmentSamplerState (IMTLSamplerState sampler, nuint index);

		[Abstract, Export ("setFragmentSamplerState:lodMinClamp:lodMaxClamp:atIndex:")]
		void SetFragmentSamplerState (IMTLSamplerState sampler, float /* float, not CGFloat */ lodMinClamp, float /* float, not CGFloat */ lodMaxClamp, nuint index);

		[Abstract, Export ("setBlendColorRed:green:blue:alpha:")]
		void SetBlendColor (float /* float, not CGFloat */ red, float /* float, not CGFloat */ green, float /* float, not CGFloat */ blue, float /* float, not CGFloat */ alpha);

		[Abstract, Export ("setDepthStencilState:")]
		void SetDepthStencilState (IMTLDepthStencilState depthStencilState);

		[Abstract, Export ("setStencilReferenceValue:")]
		void SetStencilReferenceValue (uint /* uint32_t */ referenceValue);

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("setStencilFrontReferenceValue:backReferenceValue:")]
		void SetStencilFrontReferenceValue (uint frontReferenceValue, uint backReferenceValue);

		[Abstract, Export ("setVisibilityResultMode:offset:")]
		void SetVisibilityResultMode (MTLVisibilityResultMode mode, nuint offset);

		[Abstract, Export ("drawPrimitives:vertexStart:vertexCount:instanceCount:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, nuint vertexStart, nuint vertexCount, nuint instanceCount);

		[Abstract, Export ("drawPrimitives:vertexStart:vertexCount:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, nuint vertexStart, nuint vertexCount);

		[Abstract, Export ("drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:instanceCount:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, nuint indexCount, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset, nuint instanceCount);

		[Abstract, Export ("drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, nuint indexCount, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset);

#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[iOS(9,0)]
		[Export ("drawPrimitives:vertexStart:vertexCount:instanceCount:baseInstance:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, nuint vertexStart, nuint vertexCount, nuint instanceCount, nuint baseInstance);

#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[iOS(9,0)]
		[Export ("drawIndexedPrimitives:indexCount:indexType:indexBuffer:indexBufferOffset:instanceCount:baseVertex:baseInstance:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, nuint indexCount, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset, nuint instanceCount, nint baseVertex, nuint baseInstance);

#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[iOS(9,0)]
		[Export ("drawPrimitives:indirectBuffer:indirectBufferOffset:")]
		void DrawPrimitives (MTLPrimitiveType primitiveType, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[iOS(9,0)]
		[Export ("drawIndexedPrimitives:indexType:indexBuffer:indexBufferOffset:indirectBuffer:indirectBufferOffset:")]
		void DrawIndexedPrimitives (MTLPrimitiveType primitiveType, MTLIndexType indexType, IMTLBuffer indexBuffer, nuint indexBufferOffset, IMTLBuffer indirectBuffer, nuint indirectBufferOffset);

		[Abstract, Export ("setFragmentBuffers:offsets:withRange:")]
		void SetFragmentBuffers (IMTLBuffer buffers, IntPtr IntPtrOffsets, NSRange range);

		[Abstract, Export ("setFragmentSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetFragmentSamplerStates (IMTLSamplerState [] samplers, IntPtr floatArrayPtrLodMinClamps, IntPtr floatArrayPtrLodMaxClamps, NSRange range);

		[Abstract, Export ("setFragmentSamplerStates:withRange:")]
		void SetFragmentSamplerStates (IMTLSamplerState [] samplers, NSRange range);

		[Abstract, Export ("setFragmentTextures:withRange:")]
		void SetFragmentTextures (IMTLTexture [] textures, NSRange range);

		[Abstract, Export ("setVertexBuffers:offsets:withRange:")]
		void SetVertexBuffers (IMTLBuffer [] buffers, IntPtr uintArrayPtrOffsets, NSRange range);

		[iOS (8,3)]
		[Abstract, Export ("setVertexBufferOffset:atIndex:")]
		void SetVertexBufferOffset (nuint offset, nuint index);

		[iOS (8,3)]
		[Abstract, Export ("setVertexBytes:length:atIndex:")]
		void SetVertexBytes (IntPtr bytes, nuint length, nuint index);

		[Abstract, Export ("setVertexSamplerStates:lodMinClamps:lodMaxClamps:withRange:")]
		void SetVertexSamplerStates (IMTLSamplerState [] samplers, IntPtr floatArrayPtrLodMinClamps, IntPtr floatArrayPtrLodMaxClamps, NSRange range);

		[Abstract, Export ("setVertexSamplerStates:withRange:")]
		void SetVertexSamplerStates (IMTLSamplerState [] samplers, NSRange range);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("setVertexTextures:withRange:")]
		void SetVertexTextures (IMTLTexture [] textures, NSRange range);
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLRenderPipelineColorAttachmentDescriptor : NSCopying {

		[Export ("pixelFormat")]
		MTLPixelFormat PixelFormat { get; set; }

		[Export ("blendingEnabled")]
		bool BlendingEnabled { [Bind ("isBlendingEnabled")] get; set; }

		[Export ("sourceRGBBlendFactor")]
		MTLBlendFactor SourceRgbBlendFactor { get; set; }

		[Export ("destinationRGBBlendFactor")]
		MTLBlendFactor DestinationRgbBlendFactor { get; set; }

		[Export ("rgbBlendOperation")]
		MTLBlendOperation RgbBlendOperation { get; set; }

		[Export ("sourceAlphaBlendFactor")]
		MTLBlendFactor SourceAlphaBlendFactor { get; set; }

		[Export ("destinationAlphaBlendFactor")]
		MTLBlendFactor DestinationAlphaBlendFactor { get; set; }

		[Export ("alphaBlendOperation")]
		MTLBlendOperation AlphaBlendOperation { get; set; }

		[Export ("writeMask")]
		MTLColorWriteMask WriteMask { get; set; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLRenderPipelineReflection {
		[Export ("vertexArguments")]
#if XAMCORE_4_0
		MTLArgument [] VertexArguments { get; }
#else
		NSObject [] VertexArguments { get; }
#endif

		[Export ("fragmentArguments")]
#if XAMCORE_4_0
		MTLArgument [] FragmentArguments { get; }
#else
		NSObject [] FragmentArguments { get; }
#endif
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface MTLRenderPassAttachmentDescriptor : NSCopying {

		[NullAllowed] // by default this property is null
		[Export ("texture", ArgumentSemantic.Retain)]
		IMTLTexture Texture { get; set; }

		[Export ("level")]
		nuint Level { get; set; }

		[Export ("slice")]
		nuint Slice { get; set; }

		[Export ("depthPlane")]
		nuint DepthPlane { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("resolveTexture", ArgumentSemantic.Retain)]
		IMTLTexture ResolveTexture { get; set; }

		[Export ("resolveLevel")]
		nuint ResolveLevel { get; set; }

		[Export ("resolveSlice")]
		nuint ResolveSlice { get; set; }

		[Export ("resolveDepthPlane")]
		nuint ResolveDepthPlane { get; set; }

		[Export ("loadAction")]
		MTLLoadAction LoadAction { get; set; }

		[Export ("storeAction")]
		MTLStoreAction StoreAction { get; set; }
	}
	
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (MTLRenderPassAttachmentDescriptor))]
	public interface MTLRenderPassColorAttachmentDescriptor {
		[Export ("clearColor")]
		MTLClearColor ClearColor { get; set; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (MTLRenderPassAttachmentDescriptor))]
	public interface MTLRenderPassDepthAttachmentDescriptor {

		[Export ("clearDepth")]
		double ClearDepth { get; set; }

		[iOS (9,0)]
		[NoMac]
		[Export ("depthResolveFilter")]
		MTLMultisampleDepthResolveFilter DepthResolveFilter { get; set; } 
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (MTLRenderPassAttachmentDescriptor))]
	public interface MTLRenderPassStencilAttachmentDescriptor {

		[Export ("clearStencil")]
		uint ClearStencil { get; set; } /* uint32_t */
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLRenderPassColorAttachmentDescriptorArray {
		[Export ("objectAtIndexedSubscript:"), Internal]
		MTLRenderPassColorAttachmentDescriptor ObjectAtIndexedSubscript (nuint attachmentIndex);

		[Export ("setObject:atIndexedSubscript:"), Internal]
		void SetObject (MTLRenderPassColorAttachmentDescriptor attachment, nuint attachmentIndex);
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	public interface MTLRenderPassDescriptor : NSCopying {

		[Export ("colorAttachments")]
		MTLRenderPassColorAttachmentDescriptorArray ColorAttachments { get; }

		[Export ("depthAttachment", ArgumentSemantic.Copy)]
		MTLRenderPassDepthAttachmentDescriptor DepthAttachment { get; set; }

		[Export ("stencilAttachment", ArgumentSemantic.Copy)]
		MTLRenderPassStencilAttachmentDescriptor StencilAttachment { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("visibilityResultBuffer", ArgumentSemantic.Retain)]
		IMTLBuffer VisibilityResultBuffer { get; set; }

		[Static, Export ("renderPassDescriptor")]
		[Autorelease]
		MTLRenderPassDescriptor CreateRenderPassDescriptor ();
	}

	public interface IMTLResource {}
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	[Protocol] // From Apple Docs: Your app does not define classes that implement this protocol. Model is not needed
	public partial interface MTLResource  {

		[Abstract, Export ("label")]
		string Label { get; set; }

		[Abstract, Export ("device")]
		IMTLDevice Device { get; }

		[Abstract, Export ("cpuCacheMode")]
		MTLCpuCacheMode CpuCacheMode { get; }

#if XAMCORE_4_0
		[Abstract] // new required member, but that breaks our binary compat, so we can't do that in our existing code.
#endif
		[iOS (9,0)]
		[Export ("storageMode")]
		MTLStorageMode StorageMode { get; }

		[Abstract, Export ("setPurgeableState:")]
		MTLPurgeableState SetPurgeableState (MTLPurgeableState state);
	}

	[iOS (9,0)][Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface MTLComputePipelineDescriptor : NSCopying {
		// it's marked as `nullable` but it asserts with
		// /BuildRoot/Library/Caches/com.apple.xbs/Sources/Metal/Metal-54.18/Framework/MTLComputePipeline.mm:216: failed assertion `label must not be nil.'
		[Export ("label")]
		string Label { get; set; }

		// it's marked as `nullable` but it asserts with
		// /BuildRoot/Library/Caches/com.apple.xbs/Sources/Metal/Metal-54.18/Framework/MTLComputePipeline.mm:230: failed assertion `computeFunction must not be nil.'
		[Export ("computeFunction", ArgumentSemantic.Strong)]
		IMTLFunction ComputeFunction { get; set; }

		[Export ("threadGroupSizeIsMultipleOfThreadExecutionWidth")]
		bool ThreadGroupSizeIsMultipleOfThreadExecutionWidth { get; set; }

		[Export ("reset")]
		void Reset ();
	}
}
#endif
