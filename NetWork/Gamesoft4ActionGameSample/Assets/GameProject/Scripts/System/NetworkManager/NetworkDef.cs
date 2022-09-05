using MessagePack;
using Photon.Pun;

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
            CMD_JOINROOM,                               // ルームジョイン通知
            CMD_UPDATEPLAYER_LIST,                      // プレイヤーリスト更新通知
            CMD_INPUT,                                  // キー入力通知
        }
        // ------------------------------------------------

        [MessagePackObject]
        public struct stPlayerData
        {
            [Key(0)]
            public string   name  { get; set; }
            [Key(1)]
            public int      id    { get; set; }
        }
    }
}
