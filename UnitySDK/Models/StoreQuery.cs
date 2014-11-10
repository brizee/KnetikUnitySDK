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
        }

        public void Load(Action<KnetikResult<StoreQuery>> cb)
        {
            Client.ListStorePage(PageIndex, PageSize, null, HandleResponse(cb));
        }

        public StoreQuery NextPage(Action<KnetikResult<StoreQuery>> cb = null)
        {
            var next = new StoreQuery (Client);
            next.PageIndex = PageIndex + 1;
            next.PageSize = PageSize;
            if (cb != null) {
                next.Load (cb);
            }
            return next;
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);

            Items = new List<Item> ();
            foreach (KnetikJSONNode node in json.Children) {
                Item item = Item.Parse(Client, node);
                Items.Add(item);
            }

            // JSAPI doesn't return hasMore so we keep paging until we get
            // a page with less than PageSize items.
            HasMore = Items.Count == PageSize;
        }

        protected Action<KnetikApiResponse> HandleResponse(Action<KnetikResult<StoreQuery>> cb)
        {
            return (KnetikApiResponse res) => {
                var result = new KnetikResult<StoreQuery> {
                    Response = res
                };
                if (!res.IsSuccess)
                {
                    cb(result);
                    return;
                }
                Response = res;
                
                if (res.Body["result"].Value != "null") {
                    this.Deserialize(res.Body ["result"]);
                } else {
                    Items = new List<Item> ();
                    HasMore = false;
                }
                
                result.Value = this;
                cb(result);
            };
        }
    }
}

