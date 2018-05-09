<?php
	
	include '../ConfigurationYourSharingEconomyApp.php';
    
	$id_user_to_ban = $_POST["id"];
	$ban_value = $_POST["ban"];

	BanUserFromSystem($id_user_to_ban, $ban_value);

    // Closing connection
    mysqli_close($GLOBALS['LINK_DATABASE']);
          
     //**************************************************************************************
     //**************************************************************************************
     //**************************************************************************************
     // FUNCTIONS
     //**************************************************************************************
     //**************************************************************************************
     //**************************************************************************************     
	 
     //-------------------------------------------------------------
     //  BanUserFromSystem
     //-------------------------------------------------------------
     function BanUserFromSystem($id_user_to_ban_par, $ban_value_par)
     {
		$query_update_users = "UPDATE users SET banned = $ban_value_par WHERE id = $id_user_to_ban_par";
		$result_update_users = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_users) or die("Query Error::BanUserFromSystem::Update users failed");
		 
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)		
		{
			if ($ban_value_par==1)
			{
				print "<html><body><h2>The user has been banned correctly</h2></body></html>";
			}
			else
			{
				print "<html><body><h2>The ban has been removed from the user</h2></body></html>";
			}
		}
		else
		{
			if ($ban_value_par==1)
			{
				print "<html><body><h2>Failure to apply the ban to the user</h2></body></html>";	
			}
			else
			{
				print "<html><body><h2>Failure to the ban from the user</h2></body></html>";	
			}
		}
    }	
?>
