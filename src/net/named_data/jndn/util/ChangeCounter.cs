// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2014-2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.util {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A ChangeCounter keeps a pointer to a target ChangeCountable whose change
	/// count is tracked by a local change count.  You can set to a new target which
	/// updates the local change count, and you can call checkChanged
	/// to check if the target (or one of the target's targets) has been changed.
	/// </summary>
	///
	public class ChangeCounter {
		/// <summary>
		/// Create a new ChangeCounter to track the given target. If target is not null,
		/// this sets the local change counter to target.getChangeCount().
		/// </summary>
		///
		/// <param name="target">The target to track. This may be null.</param>
		public ChangeCounter(ChangeCountable target) {
			target_ = target;
			changeCount_ = ((target_ == null) ? (long) (0) : (long) (target_.getChangeCount()));
		}
	
		/// <summary>
		/// Get the target object.  If the target is changed, then checkChanged will
		/// detect it.
		/// </summary>
		///
		/// <returns>The target object.</returns>
		public ChangeCountable get() {
			return target_;
		}
	
		/// <summary>
		/// Set the target to the given target.  If target is not null, this sets the
		/// local change counter to target.getChangeCount().
		/// </summary>
		///
		/// <param name="target">The target to track. This may be null.</param>
		public void set(ChangeCountable target) {
			target_ = target;
			changeCount_ = ((target_ == null) ? (long) (0) : (long) (target_.getChangeCount()));
		}
	
		/// <summary>
		/// If the target's change count is different than the local change count, then
		/// update the local change count and return true.  Otherwise return false,
		/// meaning that the target has not changed. Also, if the target is null,
		/// simply return false.This is useful since the target (or one of the target's
		/// targets) may be changed and you need to find out.
		/// </summary>
		///
		/// <returns>True if the change count has been updated, false if not.</returns>
		public bool checkChanged() {
			if (target_ == null)
				return false;
	
			long targetChangeCount = target_.getChangeCount();
			if (changeCount_ != targetChangeCount) {
				changeCount_ = targetChangeCount;
				return true;
			} else
				return false;
		}
	
		private ChangeCountable target_;
		private long changeCount_;
	}
}
