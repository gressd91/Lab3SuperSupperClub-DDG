using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Lab3SuperSupperClub.Models;

namespace Lab3SuperSupperClub.Models;

public class BusinessLogic : IBusinessLogic
{
    private readonly IDatabase _database;
    public ObservableCollection<SupperClub> SupperClubs { get; set; }


    public BusinessLogic(IDatabase database)
    {
        _database = database;
        SupperClubs = new ObservableCollection<SupperClub>();
        LoadSupperClubsAsync();

    }


    private async void LoadSupperClubsAsync()
    {
        var supperClubs = await GetSupperClubs();

        SupperClubs.Clear();
        foreach (var supperClub in supperClubs)
        {
            SupperClubs.Add(supperClub);
        }

    }

    public async Task<ObservableCollection<SupperClub>> GetSupperClubs()
    {
        return await _database.SelectAllSupperClubs();
    }


    public async Task<SupperClubError> AddSupperClub(int clubId, string name, string city, int[] ratings)
    {
        SupperClub? existingSupperClub = await _database.SelectSupperClub(clubId);
        if (existingSupperClub != null)
        {
            return SupperClubError.DuplicateSupperClubId;
        }

        var newSupperClub = new SupperClub
        {
            ClubId = clubId,
            Name = name,
            City = city,
            Ratings = ratings
        };
        try
        {
            await _database.InsertSupperClub(newSupperClub);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ATTN: {ex.ToString()}");
        }
        SupperClubs.Add(newSupperClub);
     //   LoadSupperClubsAsync();

        return SupperClubError.None;

    }


    public async Task<SupperClubError> DeleteSupperClub(int clubId)
    {
        var supperClub = SupperClubs.FirstOrDefault(sc => sc.ClubId == clubId);
        if (supperClub == null)
        {
            return SupperClubError.SupperClubIdNotFound;
        }

        await _database.DeleteSupperClub(clubId);
        SupperClubs.Remove(supperClub);

        return SupperClubError.None;
    }

    /// <summary>
    /// Deletes all supper clubs by calling DeleteSupperClub on each club
    /// currently in the database. 
    /// </summary>
    /// <returns>SupperClubError</returns>
    public async Task<SupperClubError> DeleteAllSupperClubs()
    {
        var supperClubs = await GetSupperClubs();
        foreach (var supperClub in supperClubs)
        {
            await DeleteSupperClub(supperClub.ClubId);
        }
        return SupperClubError.None;
    }


    public async Task<SupperClubError> EditSupperClub(int clubId, string name, string city, int[] ratings)
    {
        var supperClub = SupperClubs.FirstOrDefault(sc => sc.ClubId == clubId);
        if (supperClub == null)
        {
            return SupperClubError.SupperClubIdNotFound;
        }

        supperClub.Name = name;
        supperClub.City = city;
        supperClub.Ratings = ratings;

        await _database.UpdateSupperClub(supperClub);

        LoadSupperClubsAsync(); // or maybe just leverage ObservableObject ?

        return SupperClubError.None;
    }


    public async Task<SupperClub?> FindSupperClub(int clubId)
    {
        SupperClub? sc = await _database.SelectSupperClub(clubId);

        return sc;
    }

    public async Task<int[]> CalculateStatistics()
    {
        var ratingsCount = new int[5];
        foreach (var supperClub in SupperClubs)
        {
            var averageRating = supperClub.Ratings.Average();
            ratingsCount[(int)averageRating - 1]++;
        }
        await Task.CompletedTask;
        return ratingsCount;
    }


}
