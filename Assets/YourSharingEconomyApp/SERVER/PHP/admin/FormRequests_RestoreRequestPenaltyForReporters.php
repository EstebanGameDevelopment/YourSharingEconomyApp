<?php
	
	include '../ConfigurationYourSharingEconomyApp.php';
    
	$id_request_to_restore_flagged = $_POST["id"];
	$id_user_reporters_to_penalty = $_POST["reporters"];

	RestoreRequestsAndPenaltyReporters($id_request_to_restore_flagged, $id_user_reporters_to_penalty);

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
     //  RestoreRequestsAndPenaltyReporters
     //-------------------------------------------------------------
     function RestoreRequestsAndPenaltyReporters($id_request_par, $id_user_reporters_par)
     {
		$query_update_confirm_flag = "UPDATE requests SET reported = '', flaged = 0 WHERE id = $id_request_par";
		$result_update_confirm_flag = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_confirm_flag) or die("Query Error::FormMarkedFlaggedConfirmation::Update requests failed");
				
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			$array_reporters = explode(',',$id_user_reporters_par);
			
			foreach ($array_reporters as &$bad_reporter)
			{
				$query_update_user_score_provider = "UPDATE users SET votesprovider = votesprovider + 1 WHERE id = $bad_reporter";
				$result_update_user_score_provider = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user_score_provider) or die("Query Error::RequestUpdateScore::Update users failed");
			}
			
			print "<html><body><h2>Restored as a valid request and applied a penalty to the reporters</h2></body></html>";
		}
		else
		{
			print "<html><body><h2>Failure update request table</h2></body></html>";
		}
    }	
?>
