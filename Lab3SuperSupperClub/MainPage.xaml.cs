namespace Lab3SuperSupperClub;

using Lab3SuperSupperClub.Models;

/// <summary>
/// Name: Derek Gresser
/// Date: 9/29/25
/// Description: This lab builds off previous labs by connecting to an actual
/// database service, supabase. Now viewing, adding, updating, and deleting 
/// supper clubs connects to the corresponding supabase table. 
/// Bugs: None known
/// Reflection: Overall I thought this lab helped give me some experience with 
/// connecting to supabase in C#. I found it similar to using supabase in other
/// languages, but seemed different with how it uses the SupperClub class. I 
/// don't remember having column names and primary key's declared before 
/// getter/setter methods in other languages. My main LLM use was for trying to
/// figure out build errors at the very beginning of the lab. Its only help was 
/// explaining there was an issue somewhere in a file in the Platforms folder. 
/// I decided to create a new .NET MAUI project and copy in the other needed 
/// files from the starter code which actually worked. The time I hoped it would
/// take to finish the lab was 2 hours. The time I thought it would take was
/// 3 hours. The time it actually took was 4 hours 44 min. This took longer
/// primarily due to the build errors at the very beginning. 
/// </summary>
public partial class MainPage : ContentPage
{
	const int numRatings = 3;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = MauiProgram.businessLogic;
	}

	/// <summary>
	/// Handles the adding of a supper club from the user. This also checks that
	/// the ratings, name, city are valid and present.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void HandleAdd(object sender, EventArgs args)
	{
		int clubId;
		String name = NameENT.Text;
		String city = CityENT.Text;
		int[]? ratings = null;

		if (name == null || name.Length == 0)
		{
			await DisplayAlert("Name entry is missing", "", "OK");
			return;
		}

		if (city == null || city.Length == 0)
		{
			await DisplayAlert("City entry is missing", "", "OK");
			return;
		}

		Boolean idOK = int.TryParse(ClubIdENT.Text, out clubId);
		if (!idOK)
		{
			await DisplayAlert("Addition has failed", "Club Id must be numeric", "OK");
			return;
		}


		bool ratingsOK = TryParseRatings(RatingsENT.Text, out ratings);// should be of the form 1 2 3

		if (!ratingsOK)
		{
			await DisplayAlert($"Addition has failed", "Invalid ratings. Enter 3 numbers between 1-5, e.g., 3 4 2 for ambiance, service, food", "OK");
			return;
		}

		SupperClubError error = await MauiProgram.businessLogic.AddSupperClub(clubId, name, city, ratings!);
		if (error != SupperClubError.None)
		{
			await DisplayAlert("Addition has failed", error.ToString(), "OK");
		}
	}
	
	/// <summary>
	/// Handles the edit of a supper club from the user. This also checks that
	/// the ratings, name, city are valid and present.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void HandleEdit(object sender, EventArgs args)
	{
		int clubId;
		String name = NameENT.Text;
		String city = CityENT.Text;
		int[]? ratings = null;

		if (name == null || name.Length == 0)
		{
			await DisplayAlert("Name entry is missing", "", "OK");
			return;
		}

		if (city == null || city.Length == 0)
		{
			await DisplayAlert("City entry is missing", "", "OK");
			return;
		}

		Boolean idOK = int.TryParse(ClubIdENT.Text, out clubId);
		if (!idOK)
		{
			await DisplayAlert("Editing has failed", "Club Id must be numeric", "OK");
			return;
		}


		bool ratingsOK = TryParseRatings(RatingsENT.Text, out ratings);// should be of the form 1 2 3

		if (!ratingsOK)
		{
			await DisplayAlert($"Editing has failed", "Invalid ratings. Enter 3 numbers between 1-5, e.g., 3 4 2 for ambiance, service, food", "OK");
			return;
		}
		SupperClubError error = await MauiProgram.businessLogic.EditSupperClub(clubId, name, city, ratings!);
		if (error != SupperClubError.None)
		{
			await DisplayAlert("Editing has failed", error.ToString(), "OK");
		}
	}

	/// <summary>
	/// Handles the deletion of a supper club from the user. This then calls the 
	/// DeleteSupperClub method in the businessLogic.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void HandleDelete(object sender, EventArgs args)
	{
		SupperClub? scToDelete = (sender as Button)!.BindingContext as SupperClub;

		if (scToDelete != null)
		{
			SupperClubError error = await MauiProgram.businessLogic.DeleteSupperClub(scToDelete.ClubId);
			if (error != SupperClubError.None)
			{
				await DisplayAlert("Deletion has failed", error.ToString(), "OK");
			}
		}
	}

	/// <summary>
	/// Deletes all the supper clubs in the database. Calls DeleteAllSupperClubs in
	/// the businessLogic after the user confirms in a pop-up they would like to.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void HandleDeleteAll(object sender, EventArgs args)
	{
		bool confirm = await DisplayAlert("Confirm Delete", "Do you want to delete all clubs?", "Yes", "Cancel");
		if (!confirm)
		{
			return;
		}

		SupperClubError result = await MauiProgram.businessLogic.DeleteAllSupperClubs();

		if (result != SupperClubError.None)
		{
			await DisplayAlert("Couldn't Delete All Supper Clubs", result.ToString(), "OK");
		}
	}

	/// <summary>
	/// Clears all entry elements in the main page.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public void HandleClear(object sender, EventArgs args)
	{
		ClubIdENT.Text = string.Empty;
		NameENT.Text = string.Empty;
		CityENT.Text = string.Empty;
		RatingsENT.Text = string.Empty;
	}

	/// <summary>
	/// takes a string of the form a s f and parses it into an int [] {a, s, f}
	/// returns null if parsing fails
	/// </summary>
	/// <param name="str"></param>
	/// <param name="ratings"></param>
	/// <returns>True if ratings are valid, false otherwise</returns>
	private bool TryParseRatings(String str, out int[]? ratings)
	{
		ratings = null;
		if (str == null)
		{
			return false;
		}
		String[] pieces = str.Split(); // splitting string of the form (we hope) a s f
		if (pieces.Length < numRatings)
		{ // should be 3 ratings
			return false;
		}
		try
		{
			ratings = new int[] { int.Parse(pieces[0]), int.Parse(pieces[1]), int.Parse(pieces[2]) };
		}
		catch
		{
			return false;
		}
		// Make sure ratings are between 1-5
		foreach (int rating in ratings)
		{
			if (rating > 5 || rating < 1)
			{
				return false;
			}
		}

		return true;
	}



}



