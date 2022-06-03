using AutoMapper;
using MegaStonksService.Entities.Assets;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Services
{
    public interface IAdService
    {
        List<Ad> GetAllAds();

        Ad GetSpecificAd(int adIdTOGet);

        Ad CreateAd(CreateAdModel createAdModel);

        Ad RemoveAd(int adIdToRemove);

    }
    public class AdService : IAdService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AdService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public List<Ad> GetAllAds()
        {
            try
            {
                var ads = _context.Ads.ToList();
                foreach (var ad in ads)
                {
                    if (ad.ExpiryDate < DateTime.Now)
                    {   
                        _context.Ads.Remove(ad);
                    }
                }
                _context.SaveChanges();



                if (ads == null || ads.Count == 0)
                {
                    throw new AppException("AdService - No Ads to retrieve at this time");
                }

                return ads;
            }
            catch (Exception e)
            {
                throw new AppException($"AdService - Could not retrieve Ads from Database {e.Message}");
            }
        }

        public Ad GetSpecificAd(int adIdToGet)
        {
            try
            {
                var adToGet= _context.Ads.Find(adIdToGet);
                if(adToGet == null)
                {
                    throw new AppException("AdService - Could not retrieve the Specific Ad from the Database. Please check the Id");
                }
                return adToGet;
            }
            catch
            {
                throw new AppException("AdService - Could not retrieve the Specific Ad from the Database. Please check the Id");
            }
        }

        public Ad CreateAd(CreateAdModel createAdModel)
        {
            try
            {
                var adToCreate = new Ad
                {
                    Company = createAdModel.Company,
                    Title = createAdModel.Title,
                    Description = createAdModel.Description,
                    ImageUrl = createAdModel.ImageUrl,
                    UrlToLoad = createAdModel.UrlToLoad,
                    DateAdded = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(createAdModel.ExpiryIndays),
                    LastUpdated = DateTime.Now
                };

                _context.Ads.Add(adToCreate);
                _context.SaveChanges();
                
                return adToCreate;
            }
            catch
            {
                throw new AppException("AdService - Could not Create Ad in Database");
            }
        }

        public Ad RemoveAd(int adIdToRemove)
        {
            try
            {
                var adToRemove = _context.Ads.Find(adIdToRemove);
                _context.Ads.Remove(adToRemove);
                _context.SaveChanges();

                return adToRemove;
            }
            catch
            {
                throw new AppException("AdService - Could not Delete Ad in Database");
            }
        }

    }
}
