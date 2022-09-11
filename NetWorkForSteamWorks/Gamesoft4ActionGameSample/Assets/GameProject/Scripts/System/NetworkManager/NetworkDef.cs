using MessagePack;
using System;
using System.Collections.Generic;
using Photon.Pun;

using UnityEngine;
namespace KdGame.Net
{
    public partial class NetworkManager : MonoBehaviourPunCallbacks
    {
        // ネットワーク系応答タイムアウト時間(ms)
        private const int NETWORK_TIME_OUT_WAIT = 10000;
        // 1ROOM辺りのプレイヤー数
        private const int INROOM_PLAYER_MAX     = 4;

        // ネットワークエラー定義
        public enum ENETWORK_ERROR_CODE
        {
            NET_ERR_NORE,                                       // エラー無し

            NET_ERR_INITIALIZE,                                 // 初期化失敗
            NET_ERR_INITIALIZE_ALREADY,                         // 初期化済み

            NET_ERR_CREATEROOM,                                 // ROOM作成エラー

            NET_ERR_JOINLOBBY,                                  // ロビー参加エラー
            NET_ERR_JOINROOM,                                   // ROOM参加エラー

            NET_ERR_TIMEOUT,                                    // ネットワークタイムアウト
        }
        // ------------------------------------------------------------------------------------------------

        // 受信コマンド解析定義
        public enum ENETWORK_COMMAND
        {
            CMD_UPDATEPLAYER_LIST,                              // プレイヤーリスト更新通知

            CMD_COUNTDOWN,                                      // カウントダウン同期
            CMD_CREATECHARACTER,                                // キャラクター生成
            CMD_CHARACTERUPDATE,                                // キャラクター同期
            CMD_CHARACTEMOVE,                                   // キャラクター移動

            CMD_SYNCPOS,                                        // 位置同期
            CMD_SYNCKEY,                                        // キー同期
            CMD_SYNCATTACK,                                     // 攻撃同期
            CMD_SYNCDEAD,                                       // 戦闘不能同期
        }
        // ------------------------------------------------------------------------------------------------

        // 受信データ
        private struct stReceiveData
        {
            public ENETWORK_COMMAND     cmd     { get; set; }   // ネットワーク経由で実行されるコマンド
            public byte[]               data    { get; set; }   // 実行時利用するデータ
        }
        // ------------------------------------------------------------------------------------------------

        // ネットワーク用ルーム内情報
        [MessagePackObject]
        [Serializable]
        public struct stNetData
        {
            [Key(0)]
            public string name  { get; set; }   // ルーム内プレイヤー名
            [Key(1)]
            public int id       { get; set; }   // ルーム内プレイヤー識別ID

            public stNetData(int aID = -1, string aName = "No Name")
            {
                name    = aName;
                id      = aID;
            }
        }

        [MessagePackObject]
        [Serializable]
        public struct stNetInfo
        {
            [Key(0)]
            public List<stNetData> infolist { get; set; }
        }
    /*
            [MessagePackObject]
            [Serializable]
            public struct stPlayerData
            {
                [Key(0)]
                public string   playername                  { get; set; }

                [Key(1)]
                public int      playerid                    { get; set; }
            }

            [MessagePackObject]
            [Serializable]
            public struct stPlayerInfo
            {
                [Key(0)]
                public int                  viewid          { get; set; }

                [Key(1)]
                public List<stPlayerData>   playerlist      { get; set; }
            }
    */
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
            public int hp { get; set; }

            [Key(2)]
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

        [MessagePackObject]
        [Serializable]
        public struct stSyncAttack
        {
            // プレイヤーID
            [Key(0)]
            public int playerid { get; set; }

            [Key(1)]
            public bool attack { get; set; }
        }
    }
}
