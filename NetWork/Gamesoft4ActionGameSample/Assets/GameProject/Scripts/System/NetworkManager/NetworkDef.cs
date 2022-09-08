using MessagePack;
using System;
using System.Collections.Generic;
using Photon.Pun;

using UnityEngine;
namespace KdGame.Net
{
    public partial class NetworkManager : MonoBehaviourPunCallbacks
    {
        // ネットワークエラー定義
        public enum ENETWORK_ERROR_CODE
        {
            ERR_NORE,                                   // エラー無し

            ERR_CONNECT,                                // Photonサーバーへのコネクトエラー
            ERR_CREATEROOM,                             // ROOM作成エラー

            ERR_JOINLOBBY,                              // ロビー参加エラー
            ERR_JOINROOM,                               // ROOM参加エラー

            ERR_TIMEOUT,                                // ネットワークタイムアウト
        }
        // ------------------------------------------------

        // 受信コマンド解析定義
        public enum ENETWORK_COMMAND
        {
            CMD_UPDATEPLAYER_LIST,                      // プレイヤーリスト更新通知

            CMD_GAMESTARTCOUNTDOWN,                     // ゲームスタートカウントダウン
            CMD_CREATECHARACTER,                        // キャラクター生成
            CMD_CHARACTERUPDATE,                        // キャラクター同期
            CMD_CHARACTEMOVE,                           // キャラクター移動

            CMD_SYNCPOS,                                // 位置同期
            CMD_SYNCKEY,                                // キー同期
            CMD_INPUT,                                  // キー入力通知
        }
        // ------------------------------------------------
        [MessagePackObject]
        [Serializable]
        public struct stPlayerData
        {
            [Key(0)]
            public string   playername                  { get; set; }

            [Key(1)]
            public int      playerid                    { get; set; }
        }
        // ------------------------------------------------

        [MessagePackObject]
        [Serializable]
        public struct stPlayerInfo
        {
            [Key(0)]
            public int                  viewid          { get; set; }

            [Key(1)]
            public List<stPlayerData>   playerlist      { get; set; }
        }

        [MessagePackObject]
        [Serializable]
        public struct stCreateCharacterParameter
        {
            [Key(0)]
            public Vector3  pos { get; set; }

            // プレイヤーID
            [Key(1)]
            public int      playerid { get; set; }

            // チームID
            [Key(2)]
            public int      teamid { get; set; }

            // 名前
            [Key(3)]
            public string   name { get; set; }

            // 体力
            [Key(4)]
            public int      hp { get; set; }
        }

        [MessagePackObject]
        [Serializable]
        public struct stNetworkParameter
        {
            // プレイヤーID
            [Key(0)]
            public int playerid { get; set; }

            [Key(1)]
            public Vector2 axis { get; set; }

            [Key(2)]
            public bool attack { get; set; }

            [Key(3)]
            public bool isgrounded { get; set; }

            [Key(4)]
            public bool isdied { get; set; }
        }

        [MessagePackObject]
        [Serializable]
        public struct stSyncPos
        {
            // プレイヤーID
            [Key(0)]
            public int playerid { get; set; }

            [Key(1)]
            public Vector3 pos  { get; set; }
        }

        [MessagePackObject]
        [Serializable]
        public struct stSyncKey
        {
            // プレイヤーID
            [Key(0)]
            public int playerid { get; set; }

            [Key(1)]
            public Vector2 key { get; set; }
        }
    }
}
