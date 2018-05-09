<?php
	
	include '../ConfigurationYourSharingEconomyApp.php';
    
	$id_request_to_confirm_flagged = $_POST["id"];
	$id_user_flagged = $_POST["user"];

	MarkConfirmationFlagged($id_request_to_confirm_flagged, $id_user_flagged);

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
     //  MarkConfirmationFlagged
     //-------------------------------------------------------------
     function MarkConfirmationFlagged($id_request_par, $id_user_par)
     {
		$query_update_confirm_flag = "UPDATE requests SET confirmflag = 1, flaged = 1 WHERE id = $id_request_par";
		$result_update_confirm_flag = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_confirm_flag) or die("Query Error::FormMarkedFlaggedConfirmation::Update requests failed");
				
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			$query_update_user_score_provider = "UPDATE users SET votesuser = votesuser + 1 WHERE id = $id_user_par";
			$result_update_user_score_provider = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user_score_provider) or die("Query Error::RequestUpdateScore::Update users failed");
		
			if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)		
			{
				print "<html><body><h2>Success to update request as a confirmed flagged</h2></body></html>";
			}
			else
			{
				print "<html><body><h2>Failure update users table</h2></body></html>";	
			}
		}
		else
		{
			print "<html><body><h2>Failure update request table</h2></body></html>";
		}
    }	
?>
