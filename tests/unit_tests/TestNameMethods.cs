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
namespace net.named_data.jndn.tests.unit_tests {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.util;
	
	public class TestNameMethods {
		// Convert the int array to a ByteBuffer.
		private static ByteBuffer toBuffer(int[] array) {
			ByteBuffer result = ILOG.J2CsMapping.NIO.ByteBuffer.allocate(array.Length);
			for (int i = 0; i < array.Length; ++i)
				result.put((byte) (array[i] & 0xff));
	
			result.flip();
			return result;
		}
	
		private static readonly ByteBuffer TEST_NAME = toBuffer(new int[] { 0x7, 0x14, // Name
				0x8, 0x5, // NameComponent
				0x6c, 0x6f, 0x63, 0x61, 0x6c, 0x8, 0x3, // NameComponent
				0x6e, 0x64, 0x6e, 0x8, 0x6, // NameComponent
				0x70, 0x72, 0x65, 0x66, 0x69, 0x78 });
	
		private static readonly ByteBuffer TEST_NAME_IMPLICIT_DIGEST = toBuffer(new int[] {
				0x7,
				0x36, // Name
				0x8,
				0x5, // NameComponent
				0x6c, 0x6f, 0x63, 0x61, 0x6c,
				0x8,
				0x3, // NameComponent
				0x6e, 0x64, 0x6e,
				0x8,
				0x6, // NameComponent
				0x70, 0x72, 0x65, 0x66, 0x69, 0x78,
				0x01,
				0x20, // ImplicitSha256DigestComponent
				0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a,
				0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15,
				0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f });
	
		private String expectedURI;
		private Name.Component comp2;
	
		public void setUp() {
			expectedURI = "/entr%C3%A9e/..../%00%01%02%03";
			comp2 = new Name.Component(new Blob(
					new int[] { 0x00, 0x01, 0x02, 0x03 }));
		}
	
		public void testUriConstructor() {
			Name name = new Name(expectedURI);
			Assert.AssertEquals("Constructed name has " + name.size()
					+ " components instead of 3", 3, name.size());
			Assert.AssertEquals("URI is incorrect", expectedURI, name.toUri());
		}
	
		public void testCopyConstructor() {
			Name name = new Name(expectedURI);
			Name name2 = new Name(name);
			Assert.AssertTrue("Name from copy constructor does not match original",
					name.equals(name2));
		}
	
		public void testGetComponent() {
			Name name = new Name(expectedURI);
			Name.Component component2 = name.get(2);
			Assert.AssertTrue("Component at index 2 is incorrect",
					comp2.equals(component2));
		}
	
		public void testAppend() {
			// could possibly split this into different tests
			String uri = "/localhost/user/folders/files/%00%0F";
			Name name = new Name(uri);
			Name name2 = new Name("/localhost").append(new Name("/user/folders/"));
			Assert.AssertEquals("Name constructed by appending names has " + name2.size()
					+ " components instead of 3", 3, name2.size());
			Assert.AssertTrue("Name constructed with append has wrong suffix", name2
					.get(2).getValue().equals(new Blob("folders")));
			name2 = name2.append("files");
			Assert.AssertEquals("Name constructed by appending string has " + name2.size()
					+ " components instead of 4", 4, name2.size());
			name2 = name2.appendSegment(15);
			Assert.AssertTrue(
					"Name constructed by appending segment has wrong segment value",
					name2.get(4).getValue()
							.equals(new Blob(new int[] { 0x00, 0x0F })));
	
			Assert.AssertTrue(
					"Name constructed with append is not equal to URI constructed name",
					name2.equals(name));
			Assert.AssertEquals("Name constructed with append has wrong URI",
					name.toUri(), name2.toUri());
		}
	
		public void testPrefix() {
			Name name = new Name("/edu/cmu/andrew/user/3498478");
			Name prefix1 = name.getPrefix(2);
			Assert.AssertEquals("Name prefix has " + prefix1.size()
					+ " components instead of 2", 2, prefix1.size());
			for (int i = 0; i < 2; ++i)
				Assert.AssertTrue(name.get(i).getValue().equals(prefix1.get(i).getValue()));
	
			Name prefix2 = name.getPrefix(100);
			Assert.AssertEquals(
					"Prefix with more components than original should stop at end of original name",
					name, prefix2);
		}
	
		public void testSubName() {
			Name name = new Name("/edu/cmu/andrew/user/3498478");
			Name subName1 = name.getSubName(0);
			Assert.AssertEquals(
					"Subname from first component does not match original name",
					name, subName1);
			Name subName2 = name.getSubName(3);
			Assert.AssertEquals("/user/3498478", subName2.toUri());
	
			Name subName3 = name.getSubName(1, 3);
			Assert.AssertEquals("/cmu/andrew/user", subName3.toUri());
	
			Name subName4 = name.getSubName(0, 100);
			Assert.AssertEquals(
					"Subname with more components than original should stop at end of original name",
					name, subName4);
	
			Name subName5 = name.getSubName(7, 2);
			Assert.AssertEquals("Subname beginning after end of name should be empty",
					new Name(), subName5);
	
			Name subName6 = name.getSubName(-1, 7);
			Assert.AssertEquals(
					"Negative subname with more components than original should stop at end of original name",
					new Name("/3498478"), subName6);
	
			Name subName7 = name.getSubName(-5, 5);
			Assert.AssertEquals("Subname from (-length) should match original name", name,
					subName7);
		}
	
		public void testClear() {
			Name name = new Name(expectedURI);
			name.clear();
			Assert.AssertTrue("Cleared name is not empty", new Name().equals(name));
		}
	
		public void testCompare() {
			Name[] names = new Name[] { new Name("/a/b/d"), new Name("/c"),
					new Name("/c/a"), new Name("/bb"), new Name("/a/b/cc") };
			Object[] expectedOrder = new Object[] { "/a/b/d", "/a/b/cc", "/c",
					"/c/a", "/bb" };
			// sort calls Name.compareTo which calls Name.compare.
			System.Array.Sort(names);
	
			ArrayList sortedURIs = new ArrayList();
			for (int i = 0; i < names.Length; ++i)
				ILOG.J2CsMapping.Collections.Collections.Add(sortedURIs,names[i].toUri());
			Assert.AssertArrayEquals("Name comparison gave incorrect order",
					ILOG.J2CsMapping.Collections.Collections.ToArray(sortedURIs), expectedOrder);
	
			// Tests from ndn-cxx name.t.cpp Compare.
			Assert.AssertEquals(new Name("/A").compare(new Name("/A")), 0);
			Assert.AssertEquals(new Name("/A").compare(new Name("/A")), 0);
			Assert.AssertTrue(new Name("/A").compare(new Name("/B")) < 0);
			Assert.AssertTrue(new Name("/B").compare(new Name("/A")) > 0);
			Assert.AssertTrue(new Name("/A").compare(new Name("/AA")) < 0);
			Assert.AssertTrue(new Name("/AA").compare(new Name("/A")) > 0);
			Assert.AssertTrue(new Name("/A").compare(new Name("/A/C")) < 0);
			Assert.AssertTrue(new Name("/A/C").compare(new Name("/A")) > 0);
	
			Assert.AssertEquals(new Name("/Z/A/Y").compare(1, 1, new Name("/A")), 0);
			Assert.AssertEquals(new Name("/Z/A/Y").compare(1, 1, new Name("/A")), 0);
			Assert.AssertTrue(new Name("/Z/A/Y").compare(1, 1, new Name("/B")) < 0);
			Assert.AssertTrue(new Name("/Z/B/Y").compare(1, 1, new Name("/A")) > 0);
			Assert.AssertTrue(new Name("/Z/A/Y").compare(1, 1, new Name("/AA")) < 0);
			Assert.AssertTrue(new Name("/Z/AA/Y").compare(1, 1, new Name("/A")) > 0);
			Assert.AssertTrue(new Name("/Z/A/Y").compare(1, 1, new Name("/A/C")) < 0);
			Assert.AssertTrue(new Name("/Z/A/C/Y").compare(1, 2, new Name("/A")) > 0);
	
			Assert.AssertEquals(new Name("/Z/A").compare(1, 9, new Name("/A")), 0);
			Assert.AssertEquals(new Name("/Z/A").compare(1, 9, new Name("/A")), 0);
			Assert.AssertTrue(new Name("/Z/A").compare(1, 9, new Name("/B")) < 0);
			Assert.AssertTrue(new Name("/Z/B").compare(1, 9, new Name("/A")) > 0);
			Assert.AssertTrue(new Name("/Z/A").compare(1, 9, new Name("/AA")) < 0);
			Assert.AssertTrue(new Name("/Z/AA").compare(1, 9, new Name("/A")) > 0);
			Assert.AssertTrue(new Name("/Z/A").compare(1, 9, new Name("/A/C")) < 0);
			Assert.AssertTrue(new Name("/Z/A/C").compare(1, 9, new Name("/A")) > 0);
	
			Assert.AssertEquals(
					new Name("/Z/A/Y").compare(1, 1, new Name("/X/A/W"), 1, 1), 0);
			Assert.AssertEquals(
					new Name("/Z/A/Y").compare(1, 1, new Name("/X/A/W"), 1, 1), 0);
			Assert.AssertTrue(new Name("/Z/A/Y").compare(1, 1, new Name("/X/B/W"), 1, 1) < 0);
			Assert.AssertTrue(new Name("/Z/B/Y").compare(1, 1, new Name("/X/A/W"), 1, 1) > 0);
			Assert.AssertTrue(new Name("/Z/A/Y").compare(1, 1, new Name("/X/AA/W"), 1, 1) < 0);
			Assert.AssertTrue(new Name("/Z/AA/Y").compare(1, 1, new Name("/X/A/W"), 1, 1) > 0);
			Assert.AssertTrue(new Name("/Z/A/Y").compare(1, 1, new Name("/X/A/C/W"), 1, 2) < 0);
			Assert.AssertTrue(new Name("/Z/A/C/Y").compare(1, 2, new Name("/X/A/W"), 1, 1) > 0);
	
			Assert.AssertEquals(new Name("/Z/A/Y").compare(1, 1, new Name("/X/A"), 1), 0);
			Assert.AssertEquals(new Name("/Z/A/Y").compare(1, 1, new Name("/X/A"), 1), 0);
			Assert.AssertTrue(new Name("/Z/A/Y").compare(1, 1, new Name("/X/B"), 1) < 0);
			Assert.AssertTrue(new Name("/Z/B/Y").compare(1, 1, new Name("/X/A"), 1) > 0);
			Assert.AssertTrue(new Name("/Z/A/Y").compare(1, 1, new Name("/X/AA"), 1) < 0);
			Assert.AssertTrue(new Name("/Z/AA/Y").compare(1, 1, new Name("/X/A"), 1) > 0);
			Assert.AssertTrue(new Name("/Z/A/Y").compare(1, 1, new Name("/X/A/C"), 1) < 0);
			Assert.AssertTrue(new Name("/Z/A/C/Y").compare(1, 2, new Name("/X/A"), 1) > 0);
		}
	
		public void testMatch() {
			Name name = new Name("/edu/cmu/andrew/user/3498478");
			Name name2 = new Name(name);
			Assert.AssertTrue("Name does not match deep copy of itself", name.match(name2));
	
			name2 = name.getPrefix(2);
			Assert.AssertTrue("Name did not match prefix", name2.match(name));
			Assert.AssertFalse("Name should not match shorter name", name.match(name2));
			Assert.AssertTrue("Empty name should always match another",
					new Name().match(name));
		}
	
		public void testGetSuccessor() {
			Assert.AssertEquals(new Name("ndn:/%00%01/%01%03"), new Name(
					"ndn:/%00%01/%01%02").getSuccessor());
			Assert.AssertEquals(new Name("ndn:/%00%01/%02%00"), new Name(
					"ndn:/%00%01/%01%FF").getSuccessor());
			Assert.AssertEquals(new Name("ndn:/%00%01/%00%00%00"), new Name(
					"ndn:/%00%01/%FF%FF").getSuccessor());
			Assert.AssertEquals(new Name("/%00"), new Name().getSuccessor());
			Assert.AssertEquals(new Name("/%00%01/%00"),
					new Name("/%00%01/...").getSuccessor());
		}
	
		public void testEncodeDecode() {
			Name name = new Name("/local/ndn/prefix");
	
			Blob encoding = name.wireEncode(net.named_data.jndn.encoding.TlvWireFormat.get());
			Assert.AssertTrue(encoding.equals(new Blob(TEST_NAME, false)));
	
			Name decodedName = new Name();
			try {
				decodedName.wireDecode(new Blob(TEST_NAME, false),
						net.named_data.jndn.encoding.TlvWireFormat.get());
			} catch (EncodingException ex) {
				Assert.Fail("Can't decode TEST_NAME");
			}
			Assert.AssertEquals(decodedName, name);
	
			// Test ImplicitSha256Digest.
			Name name2 = new Name(
					"/local/ndn/prefix/sha256digest="
							+ "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f");
	
			Blob encoding2 = name2.wireEncode(net.named_data.jndn.encoding.TlvWireFormat.get());
			Assert.AssertTrue(encoding2.equals(new Blob(TEST_NAME_IMPLICIT_DIGEST, false)));
	
			Name decodedName2 = new Name();
			try {
				decodedName2.wireDecode(new Blob(TEST_NAME_IMPLICIT_DIGEST, false),
						net.named_data.jndn.encoding.TlvWireFormat.get());
			} catch (EncodingException ex_0) {
				Assert.Fail("Can't decode TEST_NAME");
			}
			Assert.AssertEquals(decodedName2, name2);
		}
	
		public void testImplicitSha256Digest() {
			Name name = new Name();
	
			ByteBuffer digest = toBuffer(new int[] { 0x28, 0xba, 0xd4, 0xb5, 0x27,
					0x5b, 0xd3, 0x92, 0xdb, 0xb6, 0x70, 0xc7, 0x5c, 0xf0, 0xb6,
					0x6f, 0x13, 0xf7, 0x94, 0x2b, 0x21, 0xe8, 0x0f, 0x55, 0xc0,
					0xe8, 0x6b, 0x37, 0x47, 0x53, 0xa5, 0x48, 0x00, 0x00 });
	
			digest.limit(32);
			name.appendImplicitSha256Digest(new Blob(digest, true));
			name.appendImplicitSha256Digest(new Blob(digest, true)
					.getImmutableArray());
			Assert.AssertEquals(name.get(0), name.get(1));
	
			digest.limit(34);
			bool gotError = true;
			try {
				name.appendImplicitSha256Digest(new Blob(digest, true));
				gotError = false;
			} catch (Exception ex) {
			}
			if (!gotError)
				Assert.Fail("Expected error in appendImplicitSha256Digest");
	
			digest.limit(30);
			gotError = true;
			try {
				name.appendImplicitSha256Digest(new Blob(digest, true));
				gotError = false;
			} catch (Exception ex_0) {
			}
			if (!gotError)
				Assert.Fail("Expected error in appendImplicitSha256Digest");
	
			// Add name.get(2) as a generic component.
			digest.limit(32);
			name.append(new Blob(digest, true));
			Assert.AssertTrue(name.get(0).compare(name.get(2)) < 0);
			Assert.AssertTrue(name.get(0).getValue().equals(name.get(2).getValue()));
	
			// Add name.get(3) as a generic component whose first byte is greater.
			digest.position(1);
			digest.limit(33);
			name.append(new Blob(digest, true));
			Assert.AssertTrue(name.get(0).compare(name.get(3)) < 0);
	
			Assert.AssertEquals(
					"sha256digest="
							+ "28bad4b5275bd392dbb670c75cf0b66f13f7942b21e80f55c0e86b374753a548",
					name.get(0).toEscapedString());
	
			Assert.AssertEquals(true, name.get(0).isImplicitSha256Digest());
			Assert.AssertEquals(false, name.get(2).isImplicitSha256Digest());
	
			gotError = true;
			try {
				new Name("/hello/sha256digest=hmm");
				gotError = false;
			} catch (Exception ex_1) {
			}
			if (!gotError)
				Assert.Fail("Expected error in new Name from URI");
	
			// Check canonical URI encoding (lower case).
			Name name2 = new Name(
					"/hello/sha256digest="
							+ "28bad4b5275bd392dbb670c75cf0b66f13f7942b21e80f55c0e86b374753a548");
			Assert.AssertEquals(name.get(0), name2.get(1));
	
			// Check that it will accept a hex value in upper case too.
			name2 = new Name(
					"/hello/sha256digest="
							+ "28BAD4B5275BD392DBB670C75CF0B66F13F7942B21E80F55C0E86B374753A548");
			Assert.AssertEquals(name.get(0), name2.get(1));
		}
	
		public void testHashCode() {
			Name foo1 = new Name("/ndn/foo");
			Name foo2 = new Name("/ndn/foo");
	
			Assert.AssertEquals("Hash codes for same Name value are not equal",
					foo1.GetHashCode(), foo2.GetHashCode());
	
			Name bar1 = new Name("/ndn/bar");
			// Strictly speaking, it is possible for a hash collision, but unlikely.
			Assert.AssertTrue("Hash codes for different Name values are not different",
					foo1.GetHashCode() != bar1.GetHashCode());
	
			Name bar2 = new Name("/ndn");
			int beforeHashCode = bar2.GetHashCode();
			bar2.append("bar");
			Assert.AssertTrue("Hash code did not change when changing the Name object",
					beforeHashCode != bar2.GetHashCode());
			Assert.AssertEquals(
					"Hash codes for same Name value after changes are not equal",
					bar1.GetHashCode(), bar2.GetHashCode());
		}
	
		public void testTypedNameComponent() {
			int otherTypeCode = 99;
			String uri = "/ndn/" + otherTypeCode + "=value";
			Name name = new Name();
			name.append("ndn").append("value", net.named_data.jndn.ComponentType.OTHER_CODE,
					otherTypeCode);
			Assert.AssertEquals(uri, name.toUri());
	
			Name nameFromUri = new Name(uri);
			Assert.AssertEquals("value", nameFromUri.get(1).getValue().toString());
			Assert.AssertEquals(otherTypeCode, nameFromUri.get(1).getOtherTypeCode());
	
			Name decodedName = new Name();
			decodedName.wireDecode(name.wireEncode());
			Assert.AssertEquals("value", decodedName.get(1).getValue().toString());
			Assert.AssertEquals(otherTypeCode, decodedName.get(1).getOtherTypeCode());
		}
	}
}
