// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Resolvers
{
    public class GeneratedResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new GeneratedResolver();

        private GeneratedResolver()
        {
        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    Formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        private static readonly global::System.Collections.Generic.Dictionary<global::System.Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(7)
            {
                { typeof(global::System.Collections.Generic.List<global::KdGame.Net.NetworkManager.stPlayerData>), 0 },
                { typeof(global::KdGame.Net.NetworkManager.stCreateCharacterParameter), 1 },
                { typeof(global::KdGame.Net.NetworkManager.stNetworkParameter), 2 },
                { typeof(global::KdGame.Net.NetworkManager.stPlayerData), 3 },
                { typeof(global::KdGame.Net.NetworkManager.stPlayerInfo), 4 },
                { typeof(global::KdGame.Net.NetworkManager.stSyncKey), 5 },
                { typeof(global::KdGame.Net.NetworkManager.stSyncPos), 6 },
            };
        }

        internal static object GetFormatter(global::System.Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                case 0: return new global::MessagePack.Formatters.ListFormatter<global::KdGame.Net.NetworkManager.stPlayerData>();
                case 1: return new MessagePack.Formatters.KdGame.Net.NetworkManager_stCreateCharacterParameterFormatter();
                case 2: return new MessagePack.Formatters.KdGame.Net.NetworkManager_stNetworkParameterFormatter();
                case 3: return new MessagePack.Formatters.KdGame.Net.NetworkManager_stPlayerDataFormatter();
                case 4: return new MessagePack.Formatters.KdGame.Net.NetworkManager_stPlayerInfoFormatter();
                case 5: return new MessagePack.Formatters.KdGame.Net.NetworkManager_stSyncKeyFormatter();
                case 6: return new MessagePack.Formatters.KdGame.Net.NetworkManager_stSyncPosFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1649 // File name should match first type name




// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.KdGame.Net
{
    public sealed class NetworkManager_stCreateCharacterParameterFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::KdGame.Net.NetworkManager.stCreateCharacterParameter>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::KdGame.Net.NetworkManager.stCreateCharacterParameter value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector3>(formatterResolver).Serialize(ref writer, value.pos, options);
            writer.Write(value.playerid);
            writer.Write(value.teamid);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.name, options);
            writer.Write(value.hp);
        }

        public global::KdGame.Net.NetworkManager.stCreateCharacterParameter Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::KdGame.Net.NetworkManager.stCreateCharacterParameter();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.pos = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector3>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.playerid = reader.ReadInt32();
                        break;
                    case 2:
                        ____result.teamid = reader.ReadInt32();
                        break;
                    case 3:
                        ____result.name = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 4:
                        ____result.hp = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkManager_stNetworkParameterFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::KdGame.Net.NetworkManager.stNetworkParameter>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::KdGame.Net.NetworkManager.stNetworkParameter value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(5);
            writer.Write(value.playerid);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector2>(formatterResolver).Serialize(ref writer, value.axis, options);
            writer.Write(value.attack);
            writer.Write(value.isgrounded);
            writer.Write(value.isdied);
        }

        public global::KdGame.Net.NetworkManager.stNetworkParameter Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::KdGame.Net.NetworkManager.stNetworkParameter();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.playerid = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.axis = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector2>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.attack = reader.ReadBoolean();
                        break;
                    case 3:
                        ____result.isgrounded = reader.ReadBoolean();
                        break;
                    case 4:
                        ____result.isdied = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkManager_stPlayerDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::KdGame.Net.NetworkManager.stPlayerData>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::KdGame.Net.NetworkManager.stPlayerData value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Serialize(ref writer, value.playername, options);
            writer.Write(value.playerid);
        }

        public global::KdGame.Net.NetworkManager.stPlayerData Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::KdGame.Net.NetworkManager.stPlayerData();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.playername = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<string>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.playerid = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkManager_stPlayerInfoFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::KdGame.Net.NetworkManager.stPlayerInfo>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::KdGame.Net.NetworkManager.stPlayerInfo value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.viewid);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::KdGame.Net.NetworkManager.stPlayerData>>(formatterResolver).Serialize(ref writer, value.playerlist, options);
        }

        public global::KdGame.Net.NetworkManager.stPlayerInfo Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::KdGame.Net.NetworkManager.stPlayerInfo();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.viewid = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.playerlist = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::System.Collections.Generic.List<global::KdGame.Net.NetworkManager.stPlayerData>>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkManager_stSyncKeyFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::KdGame.Net.NetworkManager.stSyncKey>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::KdGame.Net.NetworkManager.stSyncKey value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.playerid);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector2>(formatterResolver).Serialize(ref writer, value.key, options);
        }

        public global::KdGame.Net.NetworkManager.stSyncKey Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::KdGame.Net.NetworkManager.stSyncKey();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.playerid = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.key = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector2>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkManager_stSyncPosFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::KdGame.Net.NetworkManager.stSyncPos>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::KdGame.Net.NetworkManager.stSyncPos value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.playerid);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector3>(formatterResolver).Serialize(ref writer, value.pos, options);
        }

        public global::KdGame.Net.NetworkManager.stSyncPos Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::KdGame.Net.NetworkManager.stSyncPos();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.playerid = reader.ReadInt32();
                        break;
                    case 1:
                        ____result.pos = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector3>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name

