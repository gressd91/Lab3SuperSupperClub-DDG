using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;
using Lab3SuperSupperClub.Models;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Gotrue;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Lab3SuperSupperClub.Models;

/// <summary>
/// Database class to handle connection to supabase. Contains methods to 
/// select, select all, add, update, and delete supper clubs. This class
/// implements the IDatabase interface.
/// </summary>
public class Database : IDatabase
{
    private Supabase.Client? supabaseClient;
    private ObservableCollection<SupperClub> supperClubs = new();
    private Task waitingForInitialization;

    /// <summary>
    /// Constructor to intialize the supabase system and then calls SelectAllSupperClubs()
    /// </summary>
    public Database()
    {
        waitingForInitialization = InitializeSupabaseSystems();
        _ = SelectAllSupperClubs();
    }

    /// <summary>
    /// Creates the supabase client with the supabase url and key
    /// </summary>
    private async Task InitializeSupabaseSystems()
    {
        // Initialize Supabase client
        var supabaseUrl = "https://ifgpzgdsteciqymoqhxx.supabase.co";
        var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImlmZ3B6Z2RzdGVjaXF5bW9xaHh4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTg5MzM1OTksImV4cCI6MjA3NDUwOTU5OX0.RR59LKVldKb8Z8nGGtcXEiAOLGTCoGtGGhIN-ohiqFU";


        supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
        await supabaseClient.InitializeAsync();

    }

    /// <summary>
    /// Selects all supper clubs currently in database. Adds them all into an
    /// ObservableCollection. 
    /// </summary>
    /// <returns>An ObservableCollection of supper clubs</returns>
    public async Task<ObservableCollection<SupperClub>> SelectAllSupperClubs()
    {
        await waitingForInitialization;
        var table = supabaseClient!.From<SupperClub>();
        var response = await table.Get(); // Retrieves all supper clubs
        supperClubs.Clear();
        // Adds all clubs into the observable collection
        foreach (SupperClub sc in response.Models)
        {
            supperClubs.Add(sc);
        }
        return supperClubs;
    }

    /// <summary>
    /// Selects a specific club in the database using clubId. 
    /// </summary>
    /// <param name="clubId"></param>
    /// <returns>SupperClub object</returns>
    public async Task<SupperClub?> SelectSupperClub(int clubId)
    {
        await waitingForInitialization; // Wait for initialization to finish
        // Select clubs where clubId matches
        var response = await supabaseClient!.From<SupperClub>().Where(supperClub => supperClub.ClubId == clubId).Get();
        return response.Models.FirstOrDefault(); // Returns the first and only club in responce

    }

    /// <summary>
    /// Inserts a supper club into the database
    /// </summary>
    /// <param name="supperClub"></param>
    /// <returns>SupperClubError</returns>
    public async Task<SupperClubError> InsertSupperClub(SupperClub supperClub)
    {
        try
        {
            // Insert supperClub into database
            await supabaseClient!.From<SupperClub>().Insert(supperClub);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ATTN: Error while inserting -- {ex.ToString()}");
            return SupperClubError.InsertionError; 
        }
        return SupperClubError.None; // Insert successful
    }

    /// <summary>
    /// Updates an existing supperClub in database
    /// </summary>
    /// <param name="supperClub"></param>
    /// <returns>SupperClubError</returns>
    public async Task<SupperClubError> UpdateSupperClub(SupperClub supperClub)
    {
        try
        {
            // Updates a supper club with new data from a new SupperClub object
            await supabaseClient!.From<SupperClub>().Update(supperClub);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ATTN: Error while updating -- {ex.ToString()}");
            return SupperClubError.UpdateError;
        }
        return SupperClubError.None; // Update successful

    }

    /// <summary>
    /// Deletes a supper club using a given clubId
    /// </summary>
    /// <param name="clubId"></param>
    /// <returns>SupperClubError</returns>
    public async Task<SupperClubError> DeleteSupperClub(int clubId)
    {
        try
        {
            // Filter on clubId equal to parameter clubId, then delete
            await supabaseClient!.From<SupperClub>().Filter("club_id", Supabase.Postgrest.Constants.Operator.Equals, clubId).Delete();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ATTN: Error while deleting -- {ex.ToString()}");
            return SupperClubError.DeleteError;
        }
        return SupperClubError.None; // Delete successful
    }

}


