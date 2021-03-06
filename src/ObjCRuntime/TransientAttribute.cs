//
// Attribute to mark properties as transient, so that we don't keep
// a managed reference to them more than absolutely required.
//
// Copyright 2013 Xamarin Inc
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
using System;

namespace XamCore.ObjCRuntime {
	[AttributeUsage (AttributeTargets.Parameter, AllowMultiple=false)]
#if XAMCORE_2_0
	sealed
#endif
	public class TransientAttribute : Attribute {
	}
}
