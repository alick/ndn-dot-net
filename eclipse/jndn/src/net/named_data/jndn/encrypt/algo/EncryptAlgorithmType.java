/**
 * Copyright (C) 2015-2019 Regents of the University of California.
 * @author: Jeff Thompson <jefft0@remap.ucla.edu>
 * @author: From ndn-group-encrypt https://github.com/named-data/ndn-group-encrypt
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * A copy of the GNU Lesser General Public License is in the file COPYING.
 */

package net.named_data.jndn.encrypt.algo;

public enum EncryptAlgorithmType {
  NONE(-1),
  // These correspond to the TLV codes.
  AesEcb(0),
  AesCbc(1),
  RsaPkcs(2),
  RsaOaep(3);

  EncryptAlgorithmType(int type)
  {
    type_ = type;
  }

  public final int
  getNumericType() { return type_; }

  private final int type_;
}
