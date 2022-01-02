using PWA_TEST.Models;
using PWA_TEST.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWA_TEST.Service
{
    public class SearchService
    {
        private SearchRepository _searchRepository;

        public SearchService()
        {
            _searchRepository = new SearchRepository();
        }

        public void create(Rootobject input)
        {

            var shoppingCart =
                _searchRepository.Get<PWA_Table>(x => x.auth == input.keys.auth && x.endpoint == input.endpoint && x.p256dh == input.keys.p256dh);
            if (shoppingCart == null)
            {
                PWA_Table pWA_Table = new PWA_Table
                {
                    p256dh = input.keys.p256dh,
                    auth = input.keys.auth,
                    endpoint = input.endpoint,
                    Create = DateTime.Now,
                    Isdelete = false,
                    Cancel = 0,
                    Update = null
                };
                _searchRepository.Create(pWA_Table);//新增
            }
            else
            {
                shoppingCart.Update = DateTime.Now;
                _searchRepository.Update(shoppingCart);//更新

            }
            _searchRepository.SaveChanges();

        }

        public void delete(Rootobject input)
        {

            var shoppingCart =
                _searchRepository.Get<PWA_Table>(x => x.auth == input.keys.auth && x.endpoint == input.endpoint && x.p256dh == input.keys.p256dh);
            if (shoppingCart != null)
            {
                shoppingCart.Isdelete = true;
                shoppingCart.Cancel = 1;
                shoppingCart.Update = DateTime.Now;
                _searchRepository.Update(shoppingCart);//更新
                _searchRepository.SaveChanges();
            }
        }
        public PWA_Table get()
        {
            var shoppingCart = _searchRepository.GetAll<PWA_Table>().ToList();
            return shoppingCart.LastOrDefault();
        }
    }
}