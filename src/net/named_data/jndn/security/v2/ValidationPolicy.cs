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
namespace net.named_data.jndn.security.v2 {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	
	/// <summary>
	/// ValidationPolicy is an abstract base class that implements a validation
	/// policy for Data and Interest packets.
	/// </summary>
	///
	public abstract class ValidationPolicy {
		public ValidationPolicy() {
			this.validator_ = null;
			this.innerPolicy_ = null;
		}
		public interface ValidationContinuation {
			void continueValidation(CertificateRequest certificateRequest,
					ValidationState state);
		}
	
		/// <summary>
		/// Set the inner policy.
		/// Multiple assignments of the inner policy will create a "chain" of linked
		/// policies. The inner policy from the latest invocation of setInnerPolicy
		/// will be at the bottom of the policy list.
		/// For example, the sequence `this.setInnerPolicy(policy1)` and
		/// `this.setInnerPolicy(policy2)`, will result in
		/// `this.innerPolicy_ == policy1`,
		/// this.innerPolicy_.innerPolicy_ == policy2', and
		/// `this.innerPolicy_.innerPolicy_.innerPolicy_ == null`.
		/// </summary>
		///
		/// <exception cref="System.ArgumentException">if the innerPolicy is null.</exception>
		public void setInnerPolicy(ValidationPolicy innerPolicy) {
			if (innerPolicy == null)
				throw new ArgumentException(
						"The innerPolicy argument cannot be null");
	
			if (validator_ != null)
				innerPolicy.setValidator(validator_);
	
			if (innerPolicy_ == null)
				innerPolicy_ = innerPolicy;
			else
				innerPolicy_.setInnerPolicy(innerPolicy);
		}
	
		/// <summary>
		/// Check if the inner policy is set.
		/// </summary>
		///
		/// <returns>True if the inner policy is set.</returns>
		public bool hasInnerPolicy() {
			return innerPolicy_ != null;
		}
	
		/// <summary>
		/// Get the inner policy. If the inner policy was not set, the behavior is
		/// undefined.
		/// </summary>
		///
		/// <returns>The inner policy.</returns>
		public ValidationPolicy getInnerPolicy() {
			return innerPolicy_;
		}
	
		/// <summary>
		/// Set the validator to which this policy is associated. This replaces any
		/// previous validator.
		/// </summary>
		///
		/// <param name="validator">The validator.</param>
		public void setValidator(Validator validator) {
			validator_ = validator;
			if (innerPolicy_ != null)
				innerPolicy_.setValidator(validator);
		}
	
		/// <summary>
		/// Check the Data packet against the policy.
		/// Your derived class must implement this.
		/// Depending on the implementation of the policy, this check can be done
		/// synchronously or asynchronously.
		/// The semantics of checkPolicy are as follows:
		/// If the packet violates the policy, then the policy should call
		/// state.fail() with an appropriate error code and error description.
		/// If the packet conforms to the policy and no further key retrievals are
		/// necessary, then the policy should call
		/// continueValidation.continueValidation(null, state).
		/// If the packet conforms to the policy and a key needs to be fetched, then
		/// the policy should call
		/// continueValidation.continueValidation({appropriate-key-request-instance}, state).
		/// </summary>
		///
		/// <param name="data">The Data packet to check.</param>
		/// <param name="state">The ValidationState of this validation.</param>
		/// <param name="continueValidation"></param>
		public abstract void checkPolicy(Data data, ValidationState state,
				ValidationPolicy.ValidationContinuation  continueValidation);
	
		/// <summary>
		/// Check the Interest against the policy.
		/// Your derived class must implement this.
		/// Depending on implementation of the policy, this check can be done
		/// synchronously or asynchronously.
		/// See the checkPolicy(Data) documentation for the semantics.
		/// </summary>
		///
		/// <param name="interest">The Interest packet to check.</param>
		/// <param name="state">The ValidationState of this validation.</param>
		/// <param name="continueValidation"></param>
		public abstract void checkPolicy(Interest interest, ValidationState state,
				ValidationPolicy.ValidationContinuation  continueValidation);
	
		/// <summary>
		/// Check the certificate against the policy.
		/// This base class implementation just calls checkPolicy(Data, ...). Your
		/// derived class may override.
		/// Depending on implementation of the policy, this check can be done
		/// synchronously or asynchronously.
		/// See the checkPolicy(Data) documentation for the semantics.
		/// </summary>
		///
		/// <param name="certificate">The certificate to check.</param>
		/// <param name="state">The ValidationState of this validation.</param>
		/// <param name="continueValidation"></param>
		public void checkCertificatePolicy(CertificateV2 certificate,
				ValidationState state, ValidationPolicy.ValidationContinuation  continueValidation) {
			checkPolicy(certificate, state, continueValidation);
		}
	
		/// <summary>
		/// Extract the KeyLocator Name from a Data packet.
		/// The Data packet must contain a KeyLocator of type KEYNAME.
		/// Otherwise, state.fail is invoked with INVALID_KEY_LOCATOR.
		/// </summary>
		///
		/// <param name="data">The Data packet with the KeyLocator.</param>
		/// <param name="state">On error, this calls state.fail and returns an empty Name.</param>
		/// <returns>The KeyLocator name, or an empty Name for failure.</returns>
		public static Name getKeyLocatorName(Data data, ValidationState state) {
			return getKeyLocatorNameFromSignature(data.getSignature(), state);
		}
	
		/// <summary>
		/// Extract the KeyLocator Name from a signed Interest.
		/// The Interest must have SignatureInfo and contain a KeyLocator of type
		/// KEYNAME. Otherwise, state.fail is invoked with INVALID_KEY_LOCATOR.
		/// </summary>
		///
		/// <param name="interest">The signed Interest with the KeyLocator.</param>
		/// <param name="state">On error, this calls state.fail and returns an empty Name.</param>
		/// <returns>The KeyLocator name, or an empty Name for failure.</returns>
		public static Name getKeyLocatorName(Interest interest,
				ValidationState state) {
			Name name = interest.getName();
			if (name.size() < 2) {
				state.fail(new ValidationError(net.named_data.jndn.security.v2.ValidationError.INVALID_KEY_LOCATOR,
						"Invalid signed Interest: name too short"));
				return new Name();
			}
	
			Signature signatureInfo;
			try {
				// TODO: Generalize the WireFormat.
				signatureInfo = net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat()
						.decodeSignatureInfoAndValue(
								interest.getName().get(-2).getValue().buf(),
								interest.getName().get(-1).getValue().buf());
			} catch (Exception ex) {
				state.fail(new ValidationError(net.named_data.jndn.security.v2.ValidationError.INVALID_KEY_LOCATOR,
						"Invalid signed Interest: " + ex));
				return new Name();
			}
	
			return getKeyLocatorNameFromSignature(signatureInfo, state);
		}
	
		/// <summary>
		/// A helper method for getKeyLocatorName.
		/// </summary>
		///
		private static Name getKeyLocatorNameFromSignature(Signature signatureInfo,
				ValidationState state) {
			if (!net.named_data.jndn.KeyLocator.canGetFromSignature(signatureInfo)) {
				state.fail(new ValidationError(net.named_data.jndn.security.v2.ValidationError.INVALID_KEY_LOCATOR,
						"KeyLocator is missing"));
				return new Name();
			}
	
			KeyLocator keyLocator = net.named_data.jndn.KeyLocator.getFromSignature(signatureInfo);
			if (keyLocator.getType() != net.named_data.jndn.KeyLocatorType.KEYNAME) {
				state.fail(new ValidationError(net.named_data.jndn.security.v2.ValidationError.INVALID_KEY_LOCATOR,
						"KeyLocator type is not Name"));
				return new Name();
			}
	
			return keyLocator.getKeyName();
		}
	
		/// <summary>
		/// Get the validator_ field, used only for testing.
		/// </summary>
		///
		public Validator getValidator_() {
			return validator_;
		}
	
		protected internal Validator validator_;
		protected internal ValidationPolicy innerPolicy_;
	}
}
