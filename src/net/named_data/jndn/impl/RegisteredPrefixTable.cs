// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.impl {
	
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A RegisteredPrefixTable is an internal class to hold a list of registered
	/// prefixes with information necessary to remove the registration later.
	/// </summary>
	///
	public class RegisteredPrefixTable {
		/// <summary>
		/// Create a new RegisteredPrefixTable with an empty table.
		/// </summary>
		///
		/// <param name="interestFilterTable"></param>
		public RegisteredPrefixTable(InterestFilterTable interestFilterTable) {
			this.table_ = new ArrayList<Entry>();
			this.removeRequests_ = new ArrayList<Int64>();
			interestFilterTable_ = interestFilterTable;
		}
	
		/// <summary>
		/// Add a new entry to the table. However, if removeRegisteredPrefix was already
		/// called with the registeredPrefixId, don't add an entry and return false.
		/// </summary>
		///
		/// <param name="registeredPrefixId">The ID from Node.getNextEntryId().</param>
		/// <param name="prefix">The name prefix.</param>
		/// <param name="relatedInterestFilterId">to 0.</param>
		/// <returns>True if added an entry, false if removeRegisteredPrefix was already
		/// called with the registeredPrefixId.</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool add(long registeredPrefixId, Name prefix,
				long relatedInterestFilterId) {
			int removeRequestIndex = removeRequests_.indexOf(registeredPrefixId);
			if (removeRequestIndex >= 0) {
				// removeRegisteredPrefix was called with the registeredPrefixId returned
				//   by registerPrefix before we got here, so don't add a registered
				//   prefix table entry.
				ILOG.J2CsMapping.Collections.Collections.RemoveAt(removeRequests_,removeRequestIndex);
				return false;
			}
	
			ILOG.J2CsMapping.Collections.Collections.Add(table_,new RegisteredPrefixTable.Entry (registeredPrefixId, prefix,
							relatedInterestFilterId));
			return true;
		}
	
		/// <summary>
		/// Remove the registered prefix entry with the registeredPrefixId from the
		/// registered prefix table. This does not affect another registered prefix with
		/// a different registeredPrefixId, even if it has the same prefix name. If an
		/// interest filter was automatically created by registerPrefix, also call
		/// interestFilterTable_.unsetInterestFilter to remove it.
		/// If there is no entry with the registeredPrefixId, do nothing.
		/// </summary>
		///
		/// <param name="registeredPrefixId">The ID returned from registerPrefix.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void removeRegisteredPrefix(
				long registeredPrefixId) {
			int count = 0;
			// Go backwards through the list so we can remove entries.
			// Remove all entries even though registeredPrefixId should be unique.
			for (int i = table_.Count - 1; i >= 0; --i) {
				RegisteredPrefixTable.Entry  entry = table_[i];
	
				if (entry.getRegisteredPrefixId() == registeredPrefixId) {
					++count;
	
					if (entry.getRelatedInterestFilterId() > 0)
						// Remove the related interest filter.
						interestFilterTable_.unsetInterestFilter(entry
								.getRelatedInterestFilterId());
	
					ILOG.J2CsMapping.Collections.Collections.RemoveAt(table_,i);
				}
			}
	
			if (count == 0)
				logger_.log(
						ILOG.J2CsMapping.Util.Logging.Level.WARNING,
						"removeRegisteredPrefix: Didn't find registeredPrefixId {0}",
						registeredPrefixId);
	
			if (count == 0) {
				// The registeredPrefixId was not found. Perhaps this has been called before
				//   the callback in registerPrefix can add to the registered prefix table.
				//   Add this removal request which will be checked before adding to the
				//   registered prefix table.
				if (removeRequests_.indexOf(registeredPrefixId) < 0)
					// Not already requested, so add the request.
					ILOG.J2CsMapping.Collections.Collections.Add(removeRequests_,registeredPrefixId);
			}
		}
	
		/// <summary>
		/// A RegisteredPrefixTable.Entry holds a registeredPrefixId and information
		/// necessary to remove the registration later. It optionally holds a related
		/// interestFilterId if the InterestFilter was set in the same registerPrefix
		/// operation.
		/// </summary>
		///
		private class Entry {
			/// <summary>
			/// Create a RegisteredPrefixTable.Entry with the given values.
			/// </summary>
			///
			/// <param name="registeredPrefixId">The ID from Node.getNextEntryId().</param>
			/// <param name="prefix">The name prefix.</param>
			/// <param name="relatedInterestFilterId">to 0.</param>
			public Entry(long registeredPrefixId, Name prefix,
					long relatedInterestFilterId) {
				registeredPrefixId_ = registeredPrefixId;
				prefix_ = prefix;
				relatedInterestFilterId_ = relatedInterestFilterId;
			}
	
			/// <summary>
			/// Get the registeredPrefixId given to the constructor.
			/// </summary>
			///
			/// <returns>The registeredPrefixId.</returns>
			public long getRegisteredPrefixId() {
				return registeredPrefixId_;
			}
	
			/// <summary>
			/// Get the name prefix given to the constructor.
			/// </summary>
			///
			/// <returns>The name prefix.</returns>
			public Name getPrefix() {
				return prefix_;
			}
	
			/// <summary>
			/// Get the related interestFilterId given to the constructor.
			/// </summary>
			///
			/// <returns>The related interestFilterId.</returns>
			public long getRelatedInterestFilterId() {
				return relatedInterestFilterId_;
			}
	
			private readonly long registeredPrefixId_;
			/// <summary>
			/// < A unique identifier for this entry so it can be deleted 
			/// </summary>
			///
			private readonly Name prefix_;
			private readonly long relatedInterestFilterId_;
		}
	
		private readonly ArrayList<Entry> table_;
		private readonly InterestFilterTable interestFilterTable_;
		private readonly ArrayList<Int64> removeRequests_;
		private static readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger
				.getLogger(typeof(RegisteredPrefixTable).FullName);
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
