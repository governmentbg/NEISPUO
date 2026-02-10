namespace SB.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

public class EnumDescriptionConverter : JsonConverterFactory
{
    private (Type underlyingType, bool isEnumerable) GetTypeInfo(Type typeToConvert)
    {
        Type underlyingType;
        bool isEnumerable;

        if (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            underlyingType = Nullable.GetUnderlyingType(typeToConvert)!;
            isEnumerable = false;
        }
        else if (typeToConvert.IsArray)
        {
            underlyingType = typeToConvert.GetElementType()!;
            isEnumerable = true;
        }
        else if (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            underlyingType = typeToConvert.GetGenericArguments()[0];
            isEnumerable = true;
        }
        else if (typeToConvert.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            underlyingType = typeToConvert
                .GetInterfaces()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GenericTypeArguments[0])
                .Single();
            isEnumerable = true;
        }
        else
        {
            underlyingType = typeToConvert;
            isEnumerable = false;
        }

        return (underlyingType, isEnumerable);
    }

    public override bool CanConvert(Type typeToConvert)
        => this.GetTypeInfo(typeToConvert).underlyingType.IsEnum;

    public override JsonConverter CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var (underlyingType, isEnumerable) = this.GetTypeInfo(typeToConvert);

        var enumConverter = (JsonConverter)Activator.CreateInstance(
            typeof(EnumDescriptionConverterInner<>).MakeGenericType(
                new Type[] { underlyingType }),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: Array.Empty<object>(),
            culture: null)!;

        if (isEnumerable)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(EnumDescriptionEnumerableConverterInner<>).MakeGenericType(
                    new Type[] { underlyingType }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { enumConverter },
                culture: null)!;
        }
        else
        {
            return enumConverter;
        }
    }

    private class EnumDescriptionConverterInner<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override TEnum Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
            => throw new NotImplementedException("Converting from an enum description is not supported.");

        public override void Write(
            Utf8JsonWriter writer,
            TEnum enumValue,
            JsonSerializerOptions options)
            => writer.WriteStringValue(((Enum)enumValue).GetEnumDescription());
    }

    private class EnumDescriptionEnumerableConverterInner<TEnum> : JsonConverter<IEnumerable> where TEnum : struct, Enum
    {
        private readonly EnumDescriptionConverterInner<TEnum> enumConverter;
        public EnumDescriptionEnumerableConverterInner(EnumDescriptionConverterInner<TEnum> enumConverter)
            => this.enumConverter = enumConverter;

        public override IEnumerable Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
            => throw new NotImplementedException("Converting from an enum description collection is not supported.");

        public override void Write(
            Utf8JsonWriter writer,
            IEnumerable enumEnumerable,
            JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (TEnum enumValue in enumEnumerable)
            {
                this.enumConverter.Write(writer, enumValue, options);
            }

            writer.WriteEndArray();
        }
    }
}
