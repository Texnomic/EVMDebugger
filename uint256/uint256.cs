﻿/**
 * Author: Jonathan LEI
 * Email: xJonathan@outlook.com
 * Github: https://github.com/xJonathanLEI
 * Created: Jan 2018
 * Last Updated: Jan 2018
 * Introduction:
 * - A simble big-endian implementation of unsigned 256-bit integers in C#
 * - The code is used in EVM
**/

using System;
using Uint256.Exceptions;

namespace Uint256
{
    public class uint256
    {
        private readonly byte[] m_bytes;

        public byte[] bytes { get { return m_bytes; } }

        public uint256()
        {
            m_bytes = new byte[32];
        }

        public uint256(byte[] bytes)
        {
            if (bytes.Length > 32)
                throw new Uint256ConstructorOverflow();
            m_bytes = bytes.LeftPadTo(32);
        }

        public uint256 exp(uint256 exponent)
        {
            uint256 ret = 1;
            for(uint256 i = 0; i < exponent; i ++)
                ret *= this;
            return ret;
        }

        // Implicit conversion from int to uint256
        public static implicit operator uint256(int sint)
        {
            /**
             * We assume they mean to use unsigned-int.
             * Say, when you call new uint(1), you really mean [0x00, ..., 0x01], not some freaky huge numbers.
            **/
            byte[] bytes = BitConverter.GetBytes(sint);
            // Little endian used by BitConverter
            Array.Reverse(bytes);
            return new uint256(bytes);
        }

        public static bool operator ==(uint256 leftOperand, uint256 rightOperand)
        {
            for (int i = 0; i < 32; i++)
                if (leftOperand.bytes[i] != rightOperand.bytes[i])
                    return false;
            return true;
        }

        public static bool operator !=(uint256 leftOperand, uint256 rightOperand)
        {
            for (int i = 0; i < 32; i++)
                if (leftOperand.bytes[i] == rightOperand.bytes[i])
                    return false;
            return true;
        }

        public static uint256 operator +(uint256 leftOperand, uint256 rightOperand)
        {
            uint256 carry = leftOperand & rightOperand;
            uint256 result = leftOperand ^ rightOperand;
            while (carry == 0)
            {
                uint256 shiftedCarry = carry << 1;
                carry = result & shiftedCarry;
                result = result ^ shiftedCarry;
            }
            return result;
        }

        public static uint256 operator -(uint256 leftOperand, uint256 rightOperand)
        {
            uint256 carry = (!leftOperand) & rightOperand;
            uint256 result = leftOperand ^ rightOperand;
            while (carry == 0)
            {
                uint256 shiftedCarry = carry << 1;
                carry = (!result) & shiftedCarry;
                result = result ^ shiftedCarry;
            }
            return result;
        }

        public static uint256 operator *(uint256 leftOperand, uint256 rightOperand)
        {
            uint256 ret = 0;
            for (uint256 i = 0; i < rightOperand; i++)
                ret += leftOperand;
            return ret;
        }

        public static uint256 operator /(uint256 leftOperand, uint256 rightOperand)
        {
            if (rightOperand == 0) throw new Uint256Division0();
            uint256 ret = 0;
            while (leftOperand >= rightOperand)
            {
                leftOperand -= rightOperand;
                ret++;
            }
            return ret;
        }

        public static uint256 operator %(uint256 leftOperand, uint256 rightOperand)
        {
            if (rightOperand == 0) throw new Uint256Division0();
            while (leftOperand >= rightOperand)
                leftOperand -= rightOperand;
            return leftOperand;
        }

        public static uint256 operator <<(uint256 leftOperand, int bits)
        {
            return new uint256(leftOperand.bytes.LEFTSHIFT(bits));
        }

        public static uint256 operator >>(uint256 leftOperand, int bits)
        {
            return new uint256(leftOperand.bytes.RIGHTSHIFT(bits));
        }

        public static uint256 operator &(uint256 leftOperand, uint256 rightOperand)
        {
            return new uint256(leftOperand.bytes.AND(rightOperand.bytes));
        }

        public static uint256 operator |(uint256 leftOperand, uint256 rightOperand)
        {
            return new uint256(leftOperand.bytes.OR(rightOperand.bytes));
        }

        public static uint256 operator ^(uint256 leftOperand, uint256 rightOperand)
        {
            return new uint256(leftOperand.bytes.XOR(rightOperand.bytes));
        }

        public static uint256 operator !(uint256 leftOperand)
        {
            return new uint256(leftOperand.bytes.NOT());
        }

        public static bool operator <(uint256 leftOperand, uint256 rightOperand)
        {
            for (int i = 0; i < leftOperand.bytes.Length; i++)
                if (leftOperand.bytes[i] < rightOperand.bytes[i])
                    return true;
            return false;
        }

        public static bool operator >(uint256 leftOperand, uint256 rightOperand)
        {
            for (int i = 0; i < leftOperand.bytes.Length; i++)
                if (leftOperand.bytes[i] > rightOperand.bytes[i])
                    return true;
            return false;
        }

        public static bool operator <=(uint256 leftOperand, uint256 rightOperand)
        {
            return !(leftOperand > rightOperand);
        }

        public static bool operator >=(uint256 leftOperand, uint256 rightOperand)
        {
            return !(leftOperand < rightOperand);
        }

        public static uint256 operator ++(uint256 leftOperand)
        {
            return leftOperand + 1;
        }
    }
}
