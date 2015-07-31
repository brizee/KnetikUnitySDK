using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class StoreQuery : KnetikModel
    {
        public int PageIndex {
            get;
            set;
        }

        public int PageSize {
            get;
            set;
        }

        public List<string> Terms {
            get;
            set;
        }

        public List<string> Related {
            get;
            set;
        }

        public bool UseCatalog
        {
            get;
            set;
        }

        public List<string> ItemTypes {
            get;
            set;
        }

        public bool HasMore {
            get;
            set;
        }

        public List<Item> Items {
            get;
            set;
        }

        public StoreQuery (KnetikClient client)
        : base(client)
        {
            PageIndex = 1;
            PageSize = 25;
            UseCatalog = true;
        }

        public KnetikResult<StoreQuery> Load(Action<KnetikResult<StoreQuery>> cb = null)
        {
            if (cb != null)
            {
                // async
                Client.ListStorePage(PageIndex, PageSize, Terms, Related, UseCatalog, HandleResponse(cb));
                return null;
            } else
            {
                // sync
                return OnLoad(Client.ListStorePage(PageIndex, PageSize, Terms, Related, UseCatalog));
            }
        }

        public StoreQuery NextPage(Action<KnetikResult<StoreQuery>> cb = null)
        {
            var next = new StoreQuery (Client);
            next.PageIndex = PageIndex + 1;
            next.PageSize = PageSize;
            next.Terms = Terms;
            next.Related = Related;
            if (cb != null) {
                next.Load (cb);
            }
            return next;
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);

            Items = new List<Item> ();
            foreach (KnetikJSONNode node in json["content"].Children) {
                Item item = Item.Parse(Client, node);
                if (ItemTypes == null || ItemTypes.Contains(item.TypeHint)) {
                    Items.Add(item);
                }
            }

            // JSAPI doesn't return hasMore so we keep paging until we get
            // a page with less than PageSize items.
            HasMore = Items.Count >= PageIndex * PageSize;
        }

        private Action<KnetikApiResponse> HandleResponse(Action<KnetikResult<StoreQuery>> cb)
        {
            return (KnetikApiResponse res) => {
                cb(OnLoad(res));
            };
        }

        private KnetikResult<StoreQuery> OnLoad(KnetikApiResponse res)
        {
            var result = new KnetikResult<StoreQuery> {
                Response = res
            };
            if (!res.IsSuccess)
            {
                return result;
            }
            Response = res;
            
            if (res.Body["result"].Value != "null") {
                this.Deserialize(res.Body ["result"]);
            } else {
                Items = new List<Item> ();
                HasMore = false;
            }
            
            result.Value = this;
            return result;
        }
    }
}

