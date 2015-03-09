using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class UserInfo : KnetikModel
    {
        private KnetikDirtyTracker dirtyTracker;
        private AchievementsQuery achievements;

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

        private string _avatarURL;
        public string AvatarURL {
            get {
                return _avatarURL;
            }
            set {
                dirtyTracker.MarkDirty("AvatarURL");
                _avatarURL = value;
            }
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

        private string _language;
        public string Language {
            get {
                return _language;
            }
            set {
                dirtyTracker.MarkDirty("Language");
                _language = value;
            }
        }

        public string Country {
            get;
            set;
        }

        public List<Wallet> Wallets {
            get;
            protected set;
        }

        public Inventory Inventory {
            get;
            protected set;
        }

        public UserInfo (KnetikClient client)
            : base(client)
        {
            dirtyTracker = new KnetikDirtyTracker ();
        }

        public void Save(Action<KnetikApiResponse> cb)
        {
            Action<KnetikApiResponse> updateLang = (r) => {
                if (dirtyTracker.IsDirty ("Language")) {
                    Client.PutUserInfo("lang", Language, cb);
                } else {
                    cb(r);
                }
            };
            if (dirtyTracker.IsDirty ("AvatarURL")) {
                Client.PutUserInfo ("avatar", AvatarURL, updateLang);
            } else {
                updateLang(null);
            }
            
        }

        public void Load(Action<KnetikResult<UserInfo>> cb)
        {
            Client.GetUserInfo ((res) => {
                var result = new KnetikResult<UserInfo> {
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

        public void LoadWithGame(int gameId, Action<KnetikResult<Game>> cb)
        {
            LoadWithGame(gameId.ToString(), cb);
        }

        public void LoadWithGame(string gameIdentifier, Action<KnetikResult<Game>> cb)
        {
            Client.GetUserInfoWithProduct (gameIdentifier, (res) => {
                var result = new KnetikResult<Game> {
                    Response = res
                };
                if (!res.IsSuccess) {
                    cb(result);
                    return;
                }
                Response = res;

                if (res.Body["result"]["product_item"].AsObject == null) {
                    result.Response.Status = KnetikApiResponse.StatusType.Error;
                    result.Response.ErrorMessage = "Item not found";
                    cb(result);
                    return;
                }

                Game game = new Game(Client);
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
            Username = json ["username"].Value;
            FullName = json ["fullname"].Value;
            MobileNumber = json ["mobile_number"].Value;
            AvatarURL = json ["avatar_url"].Value;
            FirstName = json ["first_name"].Value;
            LastName = json ["last_name"].Value;
            Token = json ["token"].Value;
            Gender = json ["gender"].Value;
            Language = json ["lang"].Value;
            Country = json ["country"].Value;
            Wallets = new List<Wallet> ();
            foreach (KnetikJSONNode node in json["wallet"].Children) {
                Wallet wallet = new Wallet(Client);
                wallet.Deserialize(node);
                Wallets.Add(wallet);
            }
            Inventory = new Inventory(Client);
            if (json ["inventory"] != null)
            {
                Inventory.Deserialize(json ["inventory"]);
            }

            // Reset the dirty tracker since we have clean data
            dirtyTracker.Reset ();
        }

        public AchievementsQuery Achievements {
            get {
                if (achievements == null) {
                    achievements = new AchievementsQuery(Client, true);
                }
                return achievements;
            }
        }
    }
}

