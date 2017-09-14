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
namespace net.named_data.jndn.security.pib {
	
	using ILOG.J2CsMapping.Collections;
	using ILOG.J2CsMapping.NIO;
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data.SqlClient;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// PibSqlite3 extends PibImpl and is used by the Pib class as an implementation
	/// of a PIB based on an SQLite3 database. All the contents in the PIB are stored
	/// in an SQLite3 database file. This provides more persistent storage than
	/// PibMemory.
	/// </summary>
	///
	public class PibSqlite3 : PibImpl {
		protected internal const String INITIALIZATION1 = "CREATE TABLE IF NOT EXISTS                         \n"
				+ "  tpmInfo(                                         \n"
				+ "    tpm_locator           BLOB                     \n"
				+ "  );                                               \n";
		protected internal const String INITIALIZATION2 = "CREATE TABLE IF NOT EXISTS                         \n"
				+ "  identities(                                      \n"
				+ "    id                    INTEGER PRIMARY KEY,     \n"
				+ "    identity              BLOB NOT NULL,           \n"
				+ "    is_default            INTEGER DEFAULT 0        \n"
				+ "  );                                               \n";
		protected internal const String INITIALIZATION3 = "CREATE UNIQUE INDEX IF NOT EXISTS                  \n"
				+ "  identityIndex ON identities(identity);           \n";
		protected internal const String INITIALIZATION4 = "CREATE TABLE IF NOT EXISTS                         \n"
				+ "  keys(                                            \n"
				+ "    id                    INTEGER PRIMARY KEY,     \n"
				+ "    identity_id           INTEGER NOT NULL,        \n"
				+ "    key_name              BLOB NOT NULL,           \n"
				+ "    key_bits              BLOB NOT NULL,           \n"
				+ "    is_default            INTEGER DEFAULT 0        \n"
				+ "  );                                               \n";
		protected internal const String INITIALIZATION5 = "CREATE UNIQUE INDEX IF NOT EXISTS                  \n"
				+ "  keyIndex ON keys(key_name);                      \n";
		protected internal const String INITIALIZATION6 = "CREATE TABLE IF NOT EXISTS                         \n"
				+ "  certificates(                                    \n"
				+ "    id                    INTEGER PRIMARY KEY,     \n"
				+ "    key_id                INTEGER NOT NULL,        \n"
				+ "    certificate_name      BLOB NOT NULL,           \n"
				+ "    certificate_data      BLOB NOT NULL,           \n"
				+ "    is_default            INTEGER DEFAULT 0        \n"
				+ "  );                                               \n";
		protected internal const String INITIALIZATION7 = "CREATE UNIQUE INDEX IF NOT EXISTS                  \n"
				+ "  certIndex ON certificates(certificate_name);     \n";
	
		/// <summary>
		/// Create a new PibSqlite3 to work with an SQLite3 file. This assumes that the
		/// database directory does not contain a PIB database of an older version.
		/// </summary>
		///
		/// <param name="databaseDirectoryPath"></param>
		/// <param name="databaseFilename"></param>
		/// <exception cref="PibImpl.Error">if initialization fails.</exception>
		public PibSqlite3(String databaseDirectoryPath, String databaseFilename) {
			this.database_ = null;
			construct(databaseDirectoryPath, databaseFilename);
		}
	
		/// <summary>
		/// Create a new PibSqlite3 to work with an SQLite3 file. This assumes that the
		/// database directory does not contain a PIB database of an older version.
		/// Use "pib.db" for the databaseFilename in the databaseDirectoryPath.
		/// </summary>
		///
		/// <param name="databaseDirectoryPath"></param>
		/// <exception cref="PibImpl.Error">if initialization fails.</exception>
		public PibSqlite3(String databaseDirectoryPath) {
			this.database_ = null;
			construct(databaseDirectoryPath, "pib.db");
		}
	
		/// <summary>
		/// Create a new PibSqlite3 to work with an SQLite3 file. This assumes that the
		/// database directory does not contain a PIB database of an older version.
		/// Use $HOME/.ndn/pib.db as the database file path. If the directory does not
		/// exist, create it.
		/// </summary>
		///
		/// <exception cref="PibImpl.Error">if initialization fails.</exception>
		public PibSqlite3() {
			this.database_ = null;
			construct("", "pib.db");
		}
	
		private void construct(String databaseDirectoryPathIn,
				String databaseFilename) {
			FileInfo databaseDirectoryPath;
			if (!databaseDirectoryPathIn.equals(""))
				databaseDirectoryPath = new FileInfo(databaseDirectoryPathIn);
			else
				databaseDirectoryPath = getDefaultDatabaseDirectoryPath();
	
			System.IO.Directory.CreateDirectory(databaseDirectoryPath.FullName);
	
			FileInfo databaseFilePath = new FileInfo(System.IO.Path.Combine(databaseDirectoryPath.FullName,databaseFilename));
	
			try {
				ILOG.J2CsMapping.Reflect.Helper.GetNativeType("org.sqlite.JDBC");
			} catch (TypeLoadException ex) {
				// We don't expect this to happen.
				ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(PibSqlite3).FullName).log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
						null, ex);
				return;
			}
	
			try {
				database_ = System.Data.SqlClient.DriverManager.getConnection("jdbc:sqlite:"
						+ databaseFilePath);
	
				Statement statement = database_.CreateCommand();
				// Use "try/finally instead of "try-with-resources" or "using" which are
				// not supported before Java 7.
				try {
					// Initialize the PIB tables.
					statement.executeUpdate(INITIALIZATION1);
					statement.executeUpdate(INITIALIZATION2);
					statement.executeUpdate(INITIALIZATION3);
					statement.executeUpdate(INITIALIZATION4);
					statement.executeUpdate(INITIALIZATION5);
					statement.executeUpdate(INITIALIZATION6);
					statement.executeUpdate(INITIALIZATION7);
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		public static String getScheme() {
			return "pib-sqlite3";
		}
	
		// TpmLocator management.
	
		/// <summary>
		/// Set the corresponding TPM information to tpmLocator. This method does not
		/// reset the contents of the PIB.
		/// </summary>
		///
		/// <param name="tpmLocator">The TPM locator string.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void setTpmLocator(String tpmLocator) {
			try {
				if (getTpmLocator().equals("")) {
					// The tpmLocator does not exist. Insert it directly.
					PreparedStatement statement = database_
							.prepareStatement("INSERT INTO tpmInfo (tpm_locator) values (?)");
					statement.setString(1, tpmLocator);
					try {
						statement.executeUpdate();
					} finally {
						statement.close();
					}
				} else {
					// Update the existing tpmLocator.
					PreparedStatement statement_0 = database_
							.prepareStatement("UPDATE tpmInfo SET tpm_locator=?");
					statement_0.setString(1, tpmLocator);
					try {
						statement_0.executeUpdate();
					} finally {
						statement_0.close();
					}
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get the TPM Locator.
		/// </summary>
		///
		/// <returns>The TPM locator string.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override String getTpmLocator() {
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT tpm_locator FROM tpmInfo");
				try {
					SqlDataReader result = statement.executeQuery();
	
					if (result.NextResult())
						return result.getString(1);
					else
						return "";
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		// Identity management.
	
		/// <summary>
		/// Check for the existence of an identity.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity.</param>
		/// <returns>True if the identity exists, otherwise false.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override bool hasIdentity(Name identityName) {
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT id FROM identities WHERE identity=?");
				statement
						.setBytes(1, identityName.wireEncode().getImmutableArray());
				try {
					SqlDataReader result = statement.executeQuery();
					return result.NextResult();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Add the identity. If the identity already exists, do nothing. If no default
		/// identity has been set, set the added identity as the default.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity to add. This copies the name.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void addIdentity(Name identityName) {
			if (!hasIdentity(identityName)) {
				try {
					PreparedStatement statement = database_
							.prepareStatement("INSERT INTO identities (identity) values (?)");
					statement.setBytes(1, identityName.wireEncode()
							.getImmutableArray());
					try {
						statement.executeUpdate();
					} finally {
						statement.close();
					}
				} catch (SQLException exception) {
					throw new PibImpl.Error("PibSqlite3: SQLite error: "
							+ exception);
				}
			}
	
			if (!hasDefaultIdentity())
				setDefaultIdentity(identityName);
		}
	
		/// <summary>
		/// Remove the identity and its related keys and certificates. If the default
		/// identity is being removed, no default identity will be selected.  If the
		/// identity does not exist, do nothing.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity to remove.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void removeIdentity(Name identityName) {
			byte[] identityBytes = identityName.wireEncode().getImmutableArray();
	
			try {
				// We don't use triggers, so manually delete from keys and certificates.
				// First get the key ids.
				ArrayList<Int32> keyIds = new ArrayList<Int32>();
	
				PreparedStatement statement = database_
						.prepareStatement("SELECT keys.id "
								+ "FROM keys JOIN identities ON keys.identity_id=identities.id "
								+ "WHERE identities.identity=?");
				statement.setBytes(1, identityBytes);
	
				try {
					SqlDataReader result = statement.executeQuery();
	
					while (result.NextResult())
						ILOG.J2CsMapping.Collections.Collections.Add(keyIds,result.getInt(1));
				} finally {
					statement.close();
				}
	
				/* foreach */
				foreach (int keyId  in  keyIds) {
					statement = database_
							.prepareStatement("DELETE FROM certificates WHERE key_id=?");
					statement.setInt(1, keyId);
					try {
						statement.executeUpdate();
					} finally {
						statement.close();
					}
				}
	
				/* foreach */
				foreach (int keyId_0  in  keyIds) {
					statement = database_
							.prepareStatement("DELETE FROM keys WHERE id=?");
					statement.setInt(1, keyId_0);
					try {
						statement.executeUpdate();
					} finally {
						statement.close();
					}
				}
	
				// Now, delete from identities.
				statement = database_
						.prepareStatement("DELETE FROM identities WHERE identity=?");
				statement.setBytes(1, identityBytes);
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Erase all certificates, keys, and identities.
		/// </summary>
		///
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void clearIdentities() {
			try {
				// We don't use triggers, so manually delete from keys and certificates.
				Statement statement = database_.CreateCommand();
				statement.executeUpdate("DELETE FROM certificates");
				statement.executeUpdate("DELETE FROM keys");
	
				// Now, delete from identities.
				statement.executeUpdate("DELETE FROM identities");
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get the names of all the identities.
		/// </summary>
		///
		/// <returns>The set of identity names. The Name objects are fresh copies.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override HashedSet<Name> getIdentities() {
			HashedSet<Name> identityNames = new HashedSet<Name>();
	
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT identity FROM identities");
	
				try {
					SqlDataReader result = statement.executeQuery();
	
					while (result.NextResult()) {
						Name name = new Name();
						try {
							name.wireDecode(new Blob(result.getBytes(1)));
						} catch (EncodingException ex) {
							throw new PibImpl.Error(
									"PibSqlite3: Error decoding name: " + ex);
						}
						ILOG.J2CsMapping.Collections.Collections.Add(identityNames,name);
					}
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
	
			return identityNames;
		}
	
		/// <summary>
		/// Set the identity with the identityName as the default identity. If the
		/// identity with identityName does not exist, then it will be created.
		/// </summary>
		///
		/// <param name="identityName">The name for the default identity. This copies the name.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void setDefaultIdentity(Name identityName) {
			try {
				byte[] identityBytes = identityName.wireEncode()
						.getImmutableArray();
	
				PreparedStatement statement;
				if (!hasIdentity(identityName)) {
					statement = database_
							.prepareStatement("INSERT INTO identities (identity) values (?)");
					statement.setBytes(1, identityBytes);
					try {
						statement.executeUpdate();
					} finally {
						statement.close();
					}
				}
	
				// We don't use a trigger, so manually reset the previous default identity.
				statement = database_
						.prepareStatement("UPDATE identities SET is_default=0 WHERE is_default=1");
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
	
				// Now set the current default identity.
				statement = database_
						.prepareStatement("UPDATE identities SET is_default=1 WHERE identity=?");
				statement.setBytes(1, identityBytes);
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get the default identity.
		/// </summary>
		///
		/// <returns>The name of the default identity, as a fresh copy.</returns>
		/// <exception cref="Pib.Error">for no default identity.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override Name getDefaultIdentity() {
			try {
				Statement statement = database_.CreateCommand();
				try {
					SqlDataReader result = statement
							.executeQuery("SELECT identity FROM identities WHERE is_default=1");
	
					if (result.NextResult()) {
						Name name = new Name();
						try {
							name.wireDecode(new Blob(result.getBytes(1)));
						} catch (EncodingException ex) {
							throw new PibImpl.Error(
									"PibSqlite3: Error decoding name: " + ex);
						}
						return name;
					} else
						throw new Pib.Error("No default identity");
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		// Key management.
	
		/// <summary>
		/// Check for the existence of a key with keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>True if the key exists, otherwise false. Return false if the
		/// identity does not exist.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override bool hasKey(Name keyName) {
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT id FROM keys WHERE key_name=?");
				statement.setBytes(1, keyName.wireEncode().getImmutableArray());
				try {
					SqlDataReader result = statement.executeQuery();
					return result.NextResult();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Add the key. If a key with the same name already exists, overwrite the key.
		/// If the identity does not exist, it will be created. If no default key for
		/// the identity has been set, then set the added key as the default for the
		/// identity.  If no default identity has been set, identity becomes the
		/// default.
		/// </summary>
		///
		/// <param name="identityName"></param>
		/// <param name="keyName">The name of the key. This copies the name.</param>
		/// <param name="key">The public key bits. This copies the array.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void addKey(Name identityName, Name keyName, ByteBuffer key) {
			// Ensure the identity exists.
			addIdentity(identityName);
	
			if (!hasKey(keyName)) {
				try {
					PreparedStatement statement = database_
							.prepareStatement("INSERT INTO keys (identity_id, key_name, key_bits) "
									+ "VALUES ((SELECT id FROM identities WHERE identity=?), ?, ?)");
					statement.setBytes(1, identityName.wireEncode()
							.getImmutableArray());
					statement.setBytes(2, keyName.wireEncode().getImmutableArray());
					statement.setBytes(3, new Blob(key, false).getImmutableArray());
	
					try {
						statement.executeUpdate();
					} finally {
						statement.close();
					}
				} catch (SQLException exception) {
					throw new PibImpl.Error("PibSqlite3: SQLite error: "
							+ exception);
				}
			} else {
				try {
					PreparedStatement statement_0 = database_
							.prepareStatement("UPDATE keys SET key_bits=? WHERE key_name=?");
					statement_0.setBytes(1, new Blob(key, false).getImmutableArray());
					statement_0.setBytes(2, keyName.wireEncode().getImmutableArray());
	
					try {
						statement_0.executeUpdate();
					} finally {
						statement_0.close();
					}
				} catch (SQLException exception_1) {
					throw new PibImpl.Error("PibSqlite3: SQLite error: "
							+ exception_1);
				}
			}
	
			if (!hasDefaultKeyOfIdentity(identityName)) {
				try {
					setDefaultKeyOfIdentity(identityName, keyName);
				} catch (Pib.Error ex) {
					throw new PibImpl.Error(
							"PibSqlite3: Error setting the default key: " + ex);
				}
			}
		}
	
		/// <summary>
		/// Remove the key with keyName and its related certificates. If the key does
		/// not exist, do nothing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void removeKey(Name keyName) {
			byte[] keyNameBytes = keyName.wireEncode().getImmutableArray();
	
			try {
				// We don't use triggers, so manually delete from certificates.
				PreparedStatement statement = database_
						.prepareStatement("DELETE FROM certificates WHERE key_id=(SELECT id FROM keys WHERE key_name=?)");
				statement.setBytes(1, keyNameBytes);
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
	
				// Now, delete from keys.
				statement = database_
						.prepareStatement("DELETE FROM keys WHERE key_name=?");
				statement.setBytes(1, keyNameBytes);
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get the key bits of a key with name keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The key bits.</returns>
		/// <exception cref="Pib.Error">if the key does not exist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override Blob getKeyBits(Name keyName) {
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT key_bits FROM keys WHERE key_name=?");
				statement.setBytes(1, keyName.wireEncode().getImmutableArray());
	
				try {
					SqlDataReader result = statement.executeQuery();
	
					if (result.NextResult())
						return new Blob(result.getBytes(1), false);
					else
						throw new Pib.Error("Key `" + keyName.toUri()
								+ "` does not exist");
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get all the key names of the identity with the name identityName. The
		/// returned key names can be used to create a KeyContainer. With a key name
		/// and a backend implementation, one can create a Key front end instance.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity.</param>
		/// <returns>The set of key names. The Name objects are fresh copies. If the
		/// identity does not exist, return an empty set.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override HashedSet<Name> getKeysOfIdentity(Name identityName) {
			HashedSet<Name> keyNames = new HashedSet<Name>();
	
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT key_name "
								+ "FROM keys JOIN identities ON keys.identity_id=identities.id "
								+ "WHERE identities.identity=?");
				statement
						.setBytes(1, identityName.wireEncode().getImmutableArray());
	
				try {
					SqlDataReader result = statement.executeQuery();
	
					while (result.NextResult()) {
						Name name = new Name();
						try {
							name.wireDecode(new Blob(result.getBytes(1)));
						} catch (EncodingException ex) {
							throw new PibImpl.Error(
									"PibSqlite3: Error decoding name: " + ex);
						}
						ILOG.J2CsMapping.Collections.Collections.Add(keyNames,name);
					}
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
	
			return keyNames;
		}
	
		/// <summary>
		/// Set the key with keyName as the default key for the identity with name
		/// identityName.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity. This copies the name.</param>
		/// <param name="keyName">The name of the key. This copies the name.</param>
		/// <exception cref="Pib.Error">if the key does not exist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void setDefaultKeyOfIdentity(Name identityName, Name keyName) {
			if (!hasKey(keyName))
				throw new Pib.Error("Key `" + keyName.toUri() + "` does not exist");
	
			try {
				// We don't use a trigger, so manually reset the previous default key.
				PreparedStatement statement = database_
						.prepareStatement("UPDATE keys SET is_default=0 WHERE is_default=1");
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
	
				// Now set the current default identity.
				statement = database_
						.prepareStatement("UPDATE keys SET is_default=1 WHERE key_name=?");
				statement.setBytes(1, keyName.wireEncode().getImmutableArray());
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get the name of the default key for the identity with name identityName.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity.</param>
		/// <returns>The name of the default key, as a fresh copy.</returns>
		/// <exception cref="Pib.Error">if there is no default key or if the identity does notexist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override Name getDefaultKeyOfIdentity(Name identityName) {
			if (!hasIdentity(identityName))
				throw new Pib.Error("Identity `" + identityName.toUri()
						+ "` does not exist");
	
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT key_name "
								+ "FROM keys JOIN identities ON keys.identity_id=identities.id "
								+ "WHERE identities.identity=? AND keys.is_default=1");
				statement
						.setBytes(1, identityName.wireEncode().getImmutableArray());
	
				try {
					SqlDataReader result = statement.executeQuery();
	
					if (result.NextResult()) {
						Name name = new Name();
						try {
							name.wireDecode(new Blob(result.getBytes(1)));
						} catch (EncodingException ex) {
							throw new PibImpl.Error(
									"PibSqlite3: Error decoding name: " + ex);
						}
						return name;
					} else
						throw new Pib.Error("No default key for identity `"
								+ identityName.toUri() + "`");
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		// Certificate management.
	
		/// <summary>
		/// Check for the existence of a certificate with name certificateName.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate.</param>
		/// <returns>True if the certificate exists, otherwise false.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override bool hasCertificate(Name certificateName) {
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT id FROM certificates WHERE certificate_name=?");
				statement.setBytes(1, certificateName.wireEncode()
						.getImmutableArray());
				try {
					SqlDataReader result = statement.executeQuery();
					return result.NextResult();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Add the certificate. If a certificate with the same name (without implicit
		/// digest) already exists, then overwrite the certificate. If the key or
		/// identity does not exist, they will be created. If no default certificate
		/// for the key has been set, then set the added certificate as the default for
		/// the key. If no default key was set for the identity, it will be set as the
		/// default key for the identity. If no default identity was selected, the
		/// certificate's identity becomes the default.
		/// </summary>
		///
		/// <param name="certificate">The certificate to add. This copies the object.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void addCertificate(CertificateV2 certificate) {
			// Ensure the key exists.
			Blob content = certificate.getContent();
			addKey(certificate.getIdentity(), certificate.getKeyName(),
					content.buf());
	
			if (!hasCertificate(certificate.getName())) {
				try {
					PreparedStatement statement = database_
							.prepareStatement("INSERT INTO certificates "
									+ "(key_id, certificate_name, certificate_data) "
									+ "VALUES ((SELECT id FROM keys WHERE key_name=?), ?, ?)");
					statement.setBytes(1, certificate.getKeyName().wireEncode()
							.getImmutableArray());
					statement.setBytes(2, certificate.getName().wireEncode()
							.getImmutableArray());
					statement.setBytes(3, certificate.wireEncode()
							.getImmutableArray());
	
					try {
						statement.executeUpdate();
					} finally {
						statement.close();
					}
				} catch (SQLException exception) {
					throw new PibImpl.Error("PibSqlite3: SQLite error: "
							+ exception);
				}
			} else {
				try {
					PreparedStatement statement_0 = database_
							.prepareStatement("UPDATE certificates SET certificate_data=? WHERE certificate_name=?");
					statement_0.setBytes(1, certificate.wireEncode()
							.getImmutableArray());
					statement_0.setBytes(2, certificate.getName().wireEncode()
							.getImmutableArray());
	
					try {
						statement_0.executeUpdate();
					} finally {
						statement_0.close();
					}
				} catch (SQLException exception_1) {
					throw new PibImpl.Error("PibSqlite3: SQLite error: "
							+ exception_1);
				}
			}
	
			if (!hasDefaultCertificateOfKey(certificate.getKeyName())) {
				try {
					setDefaultCertificateOfKey(certificate.getKeyName(),
							certificate.getName());
				} catch (Pib.Error ex) {
					throw new PibImpl.Error(
							"PibSqlite3: Error setting the default certificate: "
									+ ex);
				}
			}
		}
	
		/// <summary>
		/// Remove the certificate with name certificateName. If the certificate does
		/// not exist, do nothing.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void removeCertificate(Name certificateName) {
			try {
				PreparedStatement statement = database_
						.prepareStatement("DELETE FROM certificates WHERE certificate_name=?");
				statement.setBytes(1, certificateName.wireEncode()
						.getImmutableArray());
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get the certificate with name certificateName.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate.</param>
		/// <returns>A copy of the certificate.</returns>
		/// <exception cref="Pib.Error">if the certificate does not exist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override CertificateV2 getCertificate(Name certificateName) {
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT certificate_data FROM certificates WHERE certificate_name=?");
				statement.setBytes(1, certificateName.wireEncode()
						.getImmutableArray());
	
				try {
					SqlDataReader result = statement.executeQuery();
	
					if (result.NextResult()) {
						CertificateV2 certificate = new CertificateV2();
						try {
							certificate.wireDecode(new Blob(result.getBytes(1)));
						} catch (EncodingException ex) {
							throw new PibImpl.Error(
									"PibSqlite3: Error decoding certificate: " + ex);
						}
						return certificate;
					} else
						throw new Pib.Error("Certificate `"
								+ certificateName.toUri() + "` does not exit");
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get a list of certificate names of the key with id keyName. The returned
		/// certificate names can be used to create a CertificateContainer. With a
		/// certificate name and a backend implementation, one can obtain the
		/// certificate.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The set of certificate names. The Name objects are fresh copies. If
		/// the key does not exist, return an empty set.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override HashedSet<Name> getCertificatesOfKey(Name keyName) {
			HashedSet<Name> certNames = new HashedSet<Name>();
	
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT certificate_name "
								+ "FROM certificates JOIN keys ON certificates.key_id=keys.id "
								+ "WHERE keys.key_name=?");
				statement.setBytes(1, keyName.wireEncode().getImmutableArray());
	
				try {
					SqlDataReader result = statement.executeQuery();
	
					while (result.NextResult()) {
						Name name = new Name();
						try {
							name.wireDecode(new Blob(result.getBytes(1)));
						} catch (EncodingException ex) {
							throw new PibImpl.Error(
									"PibSqlite3: Error decoding name: " + ex);
						}
						ILOG.J2CsMapping.Collections.Collections.Add(certNames,name);
					}
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
	
			return certNames;
		}
	
		/// <summary>
		/// Set the cert with name certificateName as the default for the key with
		/// keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <param name="certificateName">The name of the certificate. This copies the name.</param>
		/// <exception cref="Pib.Error">if the certificate with name certificateName does notexist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override void setDefaultCertificateOfKey(Name keyName, Name certificateName) {
			if (!hasCertificate(certificateName))
				throw new Pib.Error("Certificate `" + certificateName.toUri()
						+ "` does not exist");
	
			try {
				// We don't use a trigger, so manually reset the previous default certificate.
				PreparedStatement statement = database_
						.prepareStatement("UPDATE certificates SET is_default=0 WHERE is_default=1");
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
	
				// Now set the current default identity.
				statement = database_
						.prepareStatement("UPDATE certificates SET is_default=1 WHERE certificate_name=?");
				statement.setBytes(1, certificateName.wireEncode()
						.getImmutableArray());
				try {
					statement.executeUpdate();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get the default certificate for the key with eyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>A copy of the default certificate.</returns>
		/// <exception cref="Pib.Error">if the default certificate does not exist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public override CertificateV2 getDefaultCertificateOfKey(Name keyName) {
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT certificate_data "
								+ "FROM certificates JOIN keys ON certificates.key_id=keys.id "
								+ "WHERE certificates.is_default=1 AND keys.key_name=?");
				statement.setBytes(1, keyName.wireEncode().getImmutableArray());
	
				try {
					SqlDataReader result = statement.executeQuery();
	
					if (result.NextResult()) {
						CertificateV2 certificate = new CertificateV2();
						try {
							certificate.wireDecode(new Blob(result.getBytes(1)));
						} catch (EncodingException ex) {
							throw new PibImpl.Error(
									"PibSqlite3: Error decoding certificate: " + ex);
						}
						return certificate;
					} else
						throw new Pib.Error("No default certificate for key `"
								+ keyName.toUri() + "`");
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		/// <summary>
		/// Get the default that the constructor uses if databaseDirectoryPath is
		/// omitted. This does not try to create the directory.
		/// </summary>
		///
		/// <returns>The default database directory path.</returns>
		public static FileInfo getDefaultDatabaseDirectoryPath() {
			return new FileInfo(System.IO.Path.Combine(net.named_data.jndn.util.Common.getHomeDirectory().FullName,".ndn"));
		}
	
		/// <summary>
		/// Get the default database file path that the constructor uses if
		/// databaseDirectoryPath and databaseFilename are omitted.
		/// </summary>
		///
		/// <returns>The default database file path.</returns>
		public static FileInfo getDefaultDatabaseFilePath() {
			return new FileInfo(System.IO.Path.Combine(getDefaultDatabaseDirectoryPath().FullName,"pib.db"));
		}
	
		private bool hasDefaultIdentity() {
			try {
				Statement statement = database_.CreateCommand();
				try {
					SqlDataReader result = statement
							.executeQuery("SELECT identity FROM identities WHERE is_default=1");
					return result.NextResult();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		private bool hasDefaultKeyOfIdentity(Name identityName) {
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT key_name "
								+ "FROM keys JOIN identities ON keys.identity_id=identities.id "
								+ "WHERE identities.identity=? AND keys.is_default=1");
				statement
						.setBytes(1, identityName.wireEncode().getImmutableArray());
				try {
					SqlDataReader result = statement.executeQuery();
					return result.NextResult();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		private bool hasDefaultCertificateOfKey(Name keyName) {
			try {
				PreparedStatement statement = database_
						.prepareStatement("SELECT certificate_data "
								+ "FROM certificates JOIN keys ON certificates.key_id=keys.id "
								+ "WHERE certificates.is_default=1 AND keys.key_name=?");
				statement.setBytes(1, keyName.wireEncode().getImmutableArray());
				try {
					SqlDataReader result = statement.executeQuery();
					return result.NextResult();
				} finally {
					statement.close();
				}
			} catch (SQLException exception) {
				throw new PibImpl.Error("PibSqlite3: SQLite error: " + exception);
			}
		}
	
		private SqlConnection database_;
	}
}