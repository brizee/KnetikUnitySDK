using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class UserInfo : KnetikModel
    {
        public int ID {
            get;
            set;
        }

        public string Email {
            get;
            set;
        }

        public string Username {
            get;
            set;
        }

        public string FullName {
            get;
            set;
        }

        public string MobileNumber {
            get;
            set;
        }

        public string AvatarURL {
            get;
            set;
        }

        public string FirstName {
            get;
            set;
        }

        public string LastName {
            get;
            set;
        }

        public string Token {
            get;
            set;
        }

        public string Gender {
            get;
            set;
        }

        public string Language {
            get;
            set;
        }

        public string Country {
            get;
            set;
        }

        public List<Wallet> Wallets {
            get;
            set;
        }

        public UserInfo (KnetikClient client)
            : base(client)
        {

        }

        public void Load(Action<KnetikResult<UserInfo>> cb)
        {
            Client.GetUserInfo ((res) => {
                var result = new KnetikResult<Game> {
                    Response = res
                };
                if (!res.IsSuccess) {
                    cb(result);
                    return;
                }
                Response = res;

                this.Deserialize(res.Body["result"]);

                result.Value = this;
                cb(result);
            });
        }

        public void FindGame(int gameId, Action<KnetikResult<Game>> cb)
        {
            Client.GetUserInfoWithProduct (gameId, (res) => {
                var result = new KnetikResult<Game> {
                    Response = res
                };
                if (!res.IsSuccess) {
                    cb(result);
                    return;
                }
                Game game = new Game(Client, gameId);
                result.Value = game;
                this.Deserialize(res.Body["result"]);
                game.Deserialize(res.Body["result"]["product_item"]);
                cb(result);
            });
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            ID = json ["id"].AsInt;
            Email = json ["email"].ToString ();
            Username = json ["username"].ToString ();
            FullName = json ["fullname"].ToString ();
            MobileNumber = json ["mobile_number"].ToString ();
            AvatarURL = json ["avatar_url"].ToString ();
            FirstName = json ["first_name"].ToString ();
            LastName = json ["last_name"].ToString ();
            Token = json ["token"].ToString ();
            Gender = json ["gender"].ToString ();
            Language = json ["lang"].ToString ();
            Country = json ["country"].ToString ();
            Wallets = new List<Wallet> ();
            foreach (KnetikJSONNode node in json["wallet"]) {
                Wallet wallet = new Wallet(Client);
                wallet.Deserialize(node);
                Wallets.Add(wallet);
            }
        }
    }
}

