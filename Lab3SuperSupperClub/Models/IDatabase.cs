using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Lab3SuperSupperClub.Models;

public interface IDatabase
{
    public Task<SupperClub?> SelectSupperClub(int clubId);
    public Task<ObservableCollection<SupperClub>> SelectAllSupperClubs();

    public Task<SupperClubError> InsertSupperClub(SupperClub supperClub);
    public Task<SupperClubError> DeleteSupperClub(int clubId);
    public Task<SupperClubError> UpdateSupperClub(SupperClub supperClub);
}