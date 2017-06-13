// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.util.regex {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	
	public class NdnRegexTopMatcher : NdnRegexMatcherBase {
		public NdnRegexTopMatcher(String expr, String expand) : base(expr, net.named_data.jndn.util.regex.NdnRegexMatcherBase.NdnRegexExprType.TOP) {
			this.primaryMatcher_ = null;
			this.secondaryMatcher_ = null;
			this.primaryBackrefManager_ = new NdnRegexBackrefManager();
			this.secondaryBackrefManager_ = new NdnRegexBackrefManager();
			this.isSecondaryUsed_ = false;
			expand_ = expand;
	
			compile();
		}
	
		public NdnRegexTopMatcher(String expr) : base(expr, net.named_data.jndn.util.regex.NdnRegexMatcherBase.NdnRegexExprType.TOP) {
			this.primaryMatcher_ = null;
			this.secondaryMatcher_ = null;
			this.primaryBackrefManager_ = new NdnRegexBackrefManager();
			this.secondaryBackrefManager_ = new NdnRegexBackrefManager();
			this.isSecondaryUsed_ = false;
			expand_ = "";
	
			compile();
		}
	
		public bool match(Name name) {
			isSecondaryUsed_ = false;
	
			ILOG.J2CsMapping.Collections.Collections.Clear(matchResult_);
	
			if (primaryMatcher_.match(name, 0, name.size())) {
				ILOG.J2CsMapping.Collections.Collections.Clear(matchResult_);
				/* foreach */
				foreach (Name.Component component  in  primaryMatcher_.getMatchResult())
					ILOG.J2CsMapping.Collections.Collections.Add(matchResult_,component);
				return true;
			} else {
				if (secondaryMatcher_ != null
						&& secondaryMatcher_.match(name, 0, name.size())) {
					ILOG.J2CsMapping.Collections.Collections.Clear(matchResult_);
					/* foreach */
					foreach (Name.Component component_0  in  secondaryMatcher_
							.getMatchResult())
						ILOG.J2CsMapping.Collections.Collections.Add(matchResult_,component_0);
					isSecondaryUsed_ = true;
					return true;
				}
	
				return false;
			}
		}
	
		public override bool match(Name name, int offset, int len) {
			return match(name);
		}
	
		public Name expand(String expandStr) {
			Name result = new Name();
	
			NdnRegexBackrefManager backrefManager = ((isSecondaryUsed_) ? secondaryBackrefManager_
					: primaryBackrefManager_);
	
			int backrefNo = backrefManager.size();
	
			String expand;
	
			if (!expandStr.equals(""))
				expand = expandStr;
			else
				expand = expand_;
	
			int[] offset = new int[] { 0 };
			while (offset[0] < expand.Length) {
				String item = getItemFromExpand(expand, offset);
				if (item[0] == '<')
					result.append(item.Substring(1,(item.Length - 1)-(1)));
	
				if (item[0] == '\\') {
					int index = Int32.Parse(item.Substring(1,(item.Length)-(1)));
	
					if (0 == index) {
						/* foreach */
						foreach (Name.Component component  in  matchResult_)
							result.append(component);
					} else if (index <= backrefNo) {
						/* foreach */
						foreach (Name.Component component_0  in  backrefManager.getBackref(
								index - 1).getMatchResult())
							result.append(component_0);
					} else
						throw new NdnRegexMatcherBase.Error(
								"Exceed the range of back reference");
				}
			}
	
			return result;
		}
	
		public Name expand() {
			return expand("");
		}
	
		public static NdnRegexTopMatcher fromName(Name name, bool hasAnchor) {
			String regexStr = "^";
	
			for (int i = 0; i < name.size(); ++i) {
				regexStr += "<";
				regexStr += convertSpecialChar(name.get(i).toEscapedString());
				regexStr += ">";
			}
	
			if (hasAnchor)
				regexStr += "$";
	
			return new NdnRegexTopMatcher(regexStr);
		}
	
		public static NdnRegexTopMatcher fromName(Name name) {
			return fromName(name, false);
		}
	
		protected internal override void compile() {
			String errMsg = "Error: RegexTopMatcher.Compile(): ";
	
			String expr = expr_;
	
			if ('$' != expr[expr.Length - 1])
				expr = expr + "<.*>*";
			else
				expr = expr.Substring(0,(expr.Length - 1)-(0));
	
			if ('^' != expr[0])
				secondaryMatcher_ = new NdnRegexPatternListMatcher("<.*>*" + expr,
						secondaryBackrefManager_);
			else
				expr = expr.Substring(1,(expr.Length)-(1));
	
			primaryMatcher_ = new NdnRegexPatternListMatcher(expr,
					primaryBackrefManager_);
		}
	
		private String getItemFromExpand(String expand, int[] offset) {
			int begin = offset[0];
	
			if (expand[offset[0]] == '\\') {
				++offset[0];
				if (offset[0] >= expand.Length)
					throw new NdnRegexMatcherBase.Error(
							"wrong format of expand string!");
	
				while (offset[0] < expand.Length
						&& expand[offset[0]] <= '9'
						&& expand[offset[0]] >= '0') {
					++offset[0];
					if (offset[0] > expand.Length)
						throw new NdnRegexMatcherBase.Error(
								"wrong format of expand string!");
				}
	
				if (offset[0] > begin + 1)
					return expand.Substring(begin,(offset[0])-(begin));
				else
					throw new NdnRegexMatcherBase.Error(
							"wrong format of expand string!");
			} else if (expand[offset[0]] == '<') {
				++offset[0];
				if (offset[0] >= expand.Length)
					throw new NdnRegexMatcherBase.Error(
							"wrong format of expand string!");
	
				int left = 1;
				int right = 0;
				while (right < left) {
					if (expand[offset[0]] == '<')
						++left;
					if (expand[offset[0]] == '>')
						++right;
	
					++offset[0];
					if (offset[0] >= expand.Length)
						throw new NdnRegexMatcherBase.Error(
								"wrong format of expand string!");
				}
	
				return expand.Substring(begin,(offset[0])-(begin));
			} else
				throw new NdnRegexMatcherBase.Error(
						"wrong format of expand string!");
		}
	
		private static String convertSpecialChar(String str) {
			String newStr = "";
			for (int i = 0; i < str.Length; ++i) {
				char c = str[i];
				if (c == '.' || c == '[' || c == '{' || c == '}' || c == '('
						|| c == ')' || c == '\\' || c == '*' || c == '+'
						|| c == '?' || c == '|' || c == '^' || c == '$') {
					newStr += '\\';
					newStr += c;
				} else
					newStr += c;
			}
	
			return newStr;
		}
	
		private readonly String expand_;
		private NdnRegexPatternListMatcher primaryMatcher_;
		private NdnRegexPatternListMatcher secondaryMatcher_;
		private readonly NdnRegexBackrefManager primaryBackrefManager_;
		private readonly NdnRegexBackrefManager secondaryBackrefManager_;
		private bool isSecondaryUsed_;
	}
}
