using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.Models;
using Model.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Services
{
    public interface ITvMazeService
    {
        Task<bool> GetTvMazeResults();
        Task<List<TvShowModel>> GetDatabaseTvMazeResults(int page, int size);
    }

    public class TvMazeService : ITvMazeService
    {
        private static readonly string tvShowsUrl = "http://api.tvmaze.com/shows?page="; 
        private static readonly string castUrl = "http://api.tvmaze.com/shows/";
        private readonly DatabaseContext _db;
        private readonly IRequestService _requestService;

        public TvMazeService(DatabaseContext databaseContext, IRequestService requestService)
        {
            _db = databaseContext;
            _requestService = requestService;
        }

        //Method to get rate limited TvMaze api data 
        public async Task<bool> GetTvMazeResults()
        {
            var tvShowCount = 0;
            while (true)
            {
                var tvShowresult = await _requestService.Get<List<TvShowModel>>(new Uri(tvShowsUrl + tvShowCount.ToString()));
                if (tvShowresult.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    await Task.Delay(3000);
                    continue;
                }
                else if (!tvShowresult.IsSuccess)
                {
                    if (tvShowresult.StatusCode == System.Net.HttpStatusCode.NotFound) return true;
                    return false;
                }
                if (!tvShowresult.Data.Any())
                {
                    tvShowCount++;
                    continue;
                }
                await _db.TvShows.AddRangeAsync(tvShowresult.Data.Select(a => new DbModels.TvShow { Id = a.Id, Name = a.Name }));
                await _db.SaveChangesAsync();

                var castCount = 0;
                while (castCount < tvShowresult.Data.Count)
                {
                    var castResult = await _requestService.Get<List<CastModel>>(new Uri(castUrl + tvShowresult.Data.Skip(castCount).FirstOrDefault().Id.ToString() + "/cast"));
                    if (castResult.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        await Task.Delay(3000);
                        continue;
                    }
                    else if (!castResult.IsSuccess)
                    {
                        return false;
                    }
                    if (!castResult.Data.Any())
                    {
                        castCount++;
                        continue;
                    }
                    await _db.Cast.AddRangeAsync(castResult.Data.Select(a => new DbModels.Cast { Birthday = a.Person.Birthday, Name = a.Person.Name, TvMazeCastId = a.Person.Id, TvShowId = tvShowresult.Data.Skip(castCount).FirstOrDefault().Id }));
                    await _db.SaveChangesAsync();

                    castCount++;
                }
                tvShowCount++;
            }
        }

        //Method to get paginated TvMaze data from the database
        public async Task<List<TvShowModel>> GetDatabaseTvMazeResults(int page, int size)
        {
            var result = await _db.TvShows.AsNoTracking().Include(a => a.Cast).Skip(size*page).Take(size).Select(a => 
            new TvShowModel {
                Id = a.Id,
                Name = a.Name,
                Cast = a.Cast.Select(b => new PersonModel { Id = b.TvMazeCastId, Name = b.Name, Birthday = b.Birthday}).OrderByDescending(c => c.Birthday).ToList()
            }
            ).ToListAsync();
            return result;
        }
    }
}