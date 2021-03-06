// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2017-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security.v2.validator_config {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// ConfigFilter is an abstract base class for RegexNameFilter, etc. used by
	/// ValidatorConfig. The ValidatorConfig class consists of a set of rules.
	/// The Filter class is a part of a rule and is used to match a packet.
	/// Matched packets will be checked against the checkers defined in the rule.
	/// </summary>
	///
	public abstract class ConfigFilter {
		/// <summary>
		/// Call the virtual matchName method based on the packet type.
		/// </summary>
		///
		/// <param name="isForInterest"></param>
		/// <param name="packetName"></param>
		/// <returns>True for a match.</returns>
		public bool match(bool isForInterest, Name packetName) {
			if (isForInterest) {
				int signedInterestMinSize = 2;
	
				if (packetName.size() < signedInterestMinSize)
					return false;
	
				return matchName(packetName.getPrefix(-signedInterestMinSize));
			} else
				// Data packet.
				return matchName(packetName);
		}
	
		/// <summary>
		/// Create a filter from the configuration section.
		/// </summary>
		///
		/// <param name="configSection"></param>
		/// <returns>A new filter created from the configuration section.</returns>
		public static ConfigFilter create(BoostInfoTree configSection) {
			String filterType = configSection.getFirstValue("type");
			if (filterType == null)
				throw new ValidatorConfigError("Expected <filter.type>");
	
			if (filterType.Equals("name",StringComparison.InvariantCultureIgnoreCase))
				return createNameFilter(configSection);
			else
				throw new ValidatorConfigError("Unsupported filter.type: "
						+ filterType);
		}
	
		/// <summary>
		/// Implementation of the check for match.
		/// </summary>
		///
		/// <param name="packetName"></param>
		/// <returns>True for a match.</returns>
		protected abstract internal bool matchName(Name packetName);
	
		/// <summary>
		/// This is a helper for create() to create a filter from the configuration
		/// section which is type "name".
		/// </summary>
		///
		/// <param name="configSection">The section containing the definition of the filter.</param>
		/// <returns>A new filter created from the configuration section.</returns>
		private static ConfigFilter createNameFilter(BoostInfoTree configSection) {
			String nameUri = configSection.getFirstValue("name");
			if (nameUri != null) {
				// Get the filter.name.
				Name name = new Name(nameUri);
	
				// Get the filter.relation.
				String relationValue = configSection.getFirstValue("relation");
				if (relationValue == null)
					throw new ValidatorConfigError("Expected <filter.relation>");
	
				ConfigNameRelation.Relation relation = net.named_data.jndn.security.v2.validator_config.ConfigNameRelation
						.getNameRelationFromString(relationValue);
	
				return new ConfigRelationNameFilter(name, relation);
			}
	
			String regexString = configSection.getFirstValue("regex");
			if (regexString != null) {
				try {
					return new ConfigRegexNameFilter(regexString);
				} catch (Exception e) {
					throw new ValidatorConfigError("Wrong filter.regex: "
							+ regexString);
				}
			}
	
			throw new ValidatorConfigError("Wrong filter(name) properties");
		}
	}
}
