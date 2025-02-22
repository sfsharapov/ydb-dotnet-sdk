﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ydb.Sdk.Value
{
    public partial class YdbValue
    {
        public static YdbValue MakeInt8(sbyte value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Int8),
                new Ydb.Value
                {
                    Int32Value = value
                });
        }

        public static YdbValue MakeUint8(byte value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Uint8),
                new Ydb.Value
                {
                    Uint32Value = value
                });
        }

        public static YdbValue MakeInt16(short value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Int16),
                new Ydb.Value
                {
                    Int32Value = value
                });
        }

        public static YdbValue MakeUint16(ushort value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Uint16),
                new Ydb.Value
                {
                    Uint32Value = value
                });
        }

        public static YdbValue MakeInt32(int value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Int32),
                new Ydb.Value
                {
                    Int32Value = value
                });
        }

        public static YdbValue MakeUint32(uint value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Uint32),
                new Ydb.Value
                {
                    Uint32Value = value
                });
        }

        public static YdbValue MakeInt64(long value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Int64),
                new Ydb.Value
                {
                    Int64Value = value
                });
        }

        public static YdbValue MakeUint64(ulong value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Uint64),
                new Ydb.Value
                {
                    Uint64Value = value
                });
        }

        public static YdbValue MakeFloat(float value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Float),
                new Ydb.Value
                {
                    FloatValue = value
                });
        }

        public static YdbValue MakeDouble(double value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Double),
                new Ydb.Value
                {
                    DoubleValue = value
                });
        }

        public static YdbValue MakeDate(DateTime value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Date),
                new Ydb.Value
                {
                    Uint32Value = (uint)value.Subtract(DateTime.UnixEpoch).TotalDays
                });
        }

        public static YdbValue MakeDatetime(DateTime value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Datetime),
                new Ydb.Value
                {
                    Uint32Value = (uint)value.Subtract(DateTime.UnixEpoch).TotalSeconds
                });
        }

        public static YdbValue MakeTimestamp(DateTime value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Timestamp),
                new Ydb.Value
                {
                    Uint64Value = (ulong)(value.Subtract(DateTime.UnixEpoch).TotalMilliseconds * 1000)
                });
        }

        public static YdbValue MakeInterval(TimeSpan value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Interval),
                new Ydb.Value
                {
                    Int64Value = (long)(value.TotalMilliseconds * 1000)
                });
        }

        public static YdbValue MakeString(byte[] value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.String),
                new Ydb.Value
                {
                    BytesValue = Google.Protobuf.ByteString.CopyFrom(value)
                });
        }

        public static YdbValue MakeUtf8(string value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Utf8),
                new Ydb.Value
                {
                    TextValue = value
                });
        }

        public static YdbValue MakeYson(byte[] value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Yson),
                new Ydb.Value
                {
                    BytesValue = Google.Protobuf.ByteString.CopyFrom(value)
                });
        }

        public static YdbValue MakeJson(string value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.Json),
                new Ydb.Value
                {
                    TextValue = value
                });
        }

        public static YdbValue MakeJsonDocument(string value)
        {
            return new YdbValue(
                MakePrimitiveType(Type.Types.PrimitiveTypeId.JsonDocument),
                new Ydb.Value
                {
                    TextValue = value
                });
        }

        // TODO: EmptyOptional with complex types
        public static YdbValue MakeEmptyOptional(YdbTypeId typeId)
        {
            return new YdbValue(
                new Ydb.Type { OptionalType = new OptionalType { Item = MakePrimitiveType(typeId) } },
                new Ydb.Value { NullFlagValue = new Google.Protobuf.WellKnownTypes.NullValue() });
        }

        public static YdbValue MakeOptional(YdbValue value)
        {
            return new YdbValue(
                new Ydb.Type { OptionalType = new OptionalType { Item = value._protoType }},
                value.TypeId != YdbTypeId.OptionalType
                    ? value._protoValue
                    : new Ydb.Value { NestedValue = value._protoValue});
        }

        // TODO: MakeEmptyList with complex types
        public static YdbValue MakeEmptyList(YdbTypeId typeId)
        {
            return new YdbValue(
                new Ydb.Type { ListType = new ListType { Item = MakePrimitiveType(typeId) } },
                new Ydb.Value());
        }

        // TODO: Check items type
        public static YdbValue MakeList(IReadOnlyList<YdbValue> values)
        {
            if (values.Count == 0)
            {
                throw new ArgumentOutOfRangeException("values");
            }

            var value = new Ydb.Value();
            value.Items.Add(values.Select(v => v._protoValue));

            return new YdbValue(
                new Ydb.Type { ListType = new ListType { Item = values[0]._protoType } },
                value);
        }

        public static YdbValue MakeTuple(IReadOnlyList<YdbValue> values)
        {
            var type = new Ydb.Type()
            {
                TupleType = new TupleType()
            };

            type.TupleType.Elements.Add(values.Select(v => v._protoType));

            var value = new Ydb.Value();
            value.Items.Add(values.Select(v => v._protoValue));

            return new YdbValue(
                type,
                value);
        }

        public static YdbValue MakeStruct(IReadOnlyDictionary<string, YdbValue> members)
        {
            var type = new Ydb.Type()
            {
                StructType = new StructType()
            };

            type.StructType.Members.Add(members.Select(m => new StructMember { Name = m.Key, Type = m.Value._protoType }));

            var value = new Ydb.Value();
            value.Items.Add(members.Select(m => m.Value._protoValue));

            return new YdbValue(
                type,
                value);
        }

        private static Ydb.Type MakePrimitiveType(Type.Types.PrimitiveTypeId primitiveTypeId)
        {
            return new Ydb.Type { TypeId = primitiveTypeId };
        }

        private static Ydb.Type MakePrimitiveType(YdbTypeId typeId)
        {
            EnsurePrimitiveTypeId(typeId);
            return new Ydb.Type { TypeId = (Type.Types.PrimitiveTypeId)typeId };
        }

        private static void EnsurePrimitiveTypeId(YdbTypeId typeId)
        {
            if ((uint)typeId >= YdbTypeIdRanges.ComplexTypesFirst)
            {
                throw new ArgumentException($"Complex types aren't supported in current method: {typeId}", "typeId");
            }
        }
    }
}
