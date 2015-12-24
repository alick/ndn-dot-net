// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
// 12/23/15 3:55 PM    
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encrypt {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// GroupManagerDb is an abstract base class for the storage of data used by the
	/// GroupManager. It contains two tables to store Schedules and Members.
	/// This is an abstract base class. A subclass must implement the methods.
	/// For example, see Sqlite3GroupManagerDb.
	/// </summary>
	///
	/// @note This class is an experimental feature. The API may change.
	public abstract class GroupManagerDb {
		/// <summary>
		/// GroupManagerDb.Error extends Exception for errors using GroupManagerDb
		/// methods. Note that even though this is called "Error" to be consistent with
		/// the other libraries, it extends the Java Exception class, not Error.
		/// </summary>
		///
		[Serializable]
		public class Error : Exception {
			public Error(String message) : base(message) {
			}
		}
	
		////////////////////////////////////////////////////// Schedule management.
	
		/// <summary>
		/// Check if there is a schedule with the given name.
		/// </summary>
		///
		/// <param name="name">The name of the schedule.</param>
		/// <returns>True if there is a schedule.</returns>
		/// <exception cref="GroupManagerDb.Error">for a database error.</exception>
		public abstract bool hasSchedule(String name);
	
		/// <summary>
		/// List all the names of the schedules.
		/// </summary>
		///
		/// <returns>A new List of String with the names of all schedules. (Use List
		/// without generics so it works with older Java compilers.)</returns>
		/// <exception cref="GroupManagerDb.Error">for a database error.</exception>
		public abstract IList listAllScheduleNames();
	
		/// <summary>
		/// Get a schedule with the given name.
		/// </summary>
		///
		/// <param name="name">The name of the schedule.</param>
		/// <returns>A new Schedule object.</returns>
		/// <exception cref="GroupManagerDb.Error">if the schedule does not exist or otherdatabase error.</exception>
		public abstract Schedule getSchedule(String name);
	
		/// <summary>
		/// For each member using the given schedule, get the name and public key DER
		/// of the member's key.
		/// </summary>
		///
		/// <param name="name">The name of the schedule.</param>
		/// <returns>a new Map where the map's key is the Name of the public key and the
		/// value is the Blob of the public key DER. (Use Map without generics so it
		/// works with older Java compilers.) Note that the member's identity name is
		/// keyName.getPrefix(-1). If the schedule name is not found, the map is empty.</returns>
		/// <exception cref="GroupManagerDb.Error">for a database error.</exception>
		public abstract IDictionary getScheduleMembers(String name);
	
		/// <summary>
		/// Add a schedule with the given name.
		/// </summary>
		///
		/// <param name="name">The name of the schedule. The name cannot be empty.</param>
		/// <param name="schedule">The Schedule to add.</param>
		/// <exception cref="GroupManagerDb.Error">if a schedule with the same name already exists,if the name is empty, or other database error.</exception>
		public abstract void addSchedule(String name, Schedule schedule);
	
		/// <summary>
		/// Delete the schedule with the given name. Also delete members which use this
		/// schedule. If there is no schedule with the name, then do nothing.
		/// </summary>
		///
		/// <param name="name">The name of the schedule.</param>
		/// <exception cref="GroupManagerDb.Error">for a database error.</exception>
		public abstract void deleteSchedule(String name);
	
		/// <summary>
		/// Rename a schedule with oldName to newName.
		/// </summary>
		///
		/// <param name="oldName">The name of the schedule to be renamed.</param>
		/// <param name="newName">The new name of the schedule. The name cannot be empty.</param>
		/// <exception cref="GroupManagerDb.Error">If a schedule with newName already exists, ifthe schedule with oldName does not exist, if newName is empty, or otherdatabase error.</exception>
		public abstract void renameSchedule(String oldName, String newName);
	
		/// <summary>
		/// Update the schedule with name and replace the old object with the given
		/// schedule. Otherwise, if no schedule with name exists, a new schedule
		/// with name and the given schedule will be added to database.
		/// </summary>
		///
		/// <param name="name">The name of the schedule. The name cannot be empty.</param>
		/// <param name="schedule">The Schedule to update or add.</param>
		/// <exception cref="GroupManagerDb.Error">if the name is empty, or other database error.</exception>
		public abstract void updateSchedule(String name, Schedule schedule);
	
		////////////////////////////////////////////////////// Member management.
	
		/// <summary>
		/// Check if there is a member with the given identity name.
		/// </summary>
		///
		/// <param name="identity">The member's identity name.</param>
		/// <returns>True if there is a member.</returns>
		/// <exception cref="GroupManagerDb.Error">for a database error.</exception>
		public abstract bool hasMember(Name identity);
	
		/// <summary>
		/// List all the members.
		/// </summary>
		///
		/// <returns>A new List of Name with the names of all members. (Use List without
		/// generics so it works with older Java compilers.)</returns>
		/// <exception cref="GroupManagerDb.Error">for a database error.</exception>
		public abstract IList listAllMembers();
	
		/// <summary>
		/// Get the name of the schedule for the given member's identity name.
		/// </summary>
		///
		/// <param name="identity">The member's identity name.</param>
		/// <returns>The name of the schedule.</returns>
		/// <exception cref="GroupManagerDb.Error">if there's no member with the given identityname in the database, or other database error.</exception>
		public abstract String getMemberSchedule(Name identity);
	
		/// <summary>
		/// Add a new member with the given key named keyName into a schedule named
		/// scheduleName. The member's identity name is keyName.getPrefix(-1).
		/// </summary>
		///
		/// <param name="scheduleName">The schedule name.</param>
		/// <param name="keyName">The name of the key.</param>
		/// <param name="key">A Blob of the public key DER.</param>
		/// <exception cref="GroupManagerDb.Error">If there's no schedule named scheduleName, ifthe member's identity name already exists, or other database error.</exception>
		public abstract void addMember(String scheduleName, Name keyName, Blob key);
	
		/// <summary>
		/// Change the name of the schedule for the given member's identity name.
		/// </summary>
		///
		/// <param name="identity">The member's identity name.</param>
		/// <param name="scheduleName">The new schedule name.</param>
		/// <exception cref="GroupManagerDb.Error">if there's no member with the given identityname in the database, or there's no schedule named scheduleName, or otherdatabase error.</exception>
		public abstract void updateMemberSchedule(Name identity, String scheduleName);
	
		/// <summary>
		/// Delete a member with the given identity name. If there is no member with
		/// the identity name, then do nothing.
		/// </summary>
		///
		/// <param name="identity">The member's identity name.</param>
		/// <exception cref="GroupManagerDb.Error">for a database error.</exception>
		public abstract void deleteMember(Name identity);
	}
}
