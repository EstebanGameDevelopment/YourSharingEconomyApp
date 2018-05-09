<?php
	
	include '../ConfigurationYourSharingEconomyApp.php';
    
	$id_provider_reported = $_POST["id"];
	$id_proposal_reported = $_POST["proposal"];

	PenaltyToTheProvider($id_provider_reported, $id_proposal_reported);

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
     //  PenaltyToTheProvider
     //-------------------------------------------------------------
     function PenaltyToTheProvider($id_provider_reported_par, $id_proposal_reported_par)
     {
		$query_update_proposal = "UPDATE proposals SET confirmedreported = 1 WHERE id = $id_proposal_reported_par";
		$result_update_proposal = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_proposal) or die("Query Error::RequestUpdateScore::Update users failed");
		 
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)		
		{
			$query_update_user_score_provider = "UPDATE users SET votesprovider = votesprovider + 1 WHERE id = $id_provider_reported_par";
			$result_update_user_score_provider = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user_score_provider) or die("Query Error::RequestUpdateScore::Update users failed");

			if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)		
			{
				print "<html><body><h2>Success to apply the penalty to the provider</h2></body></html>";
			}
			else
			{
				print "<html><body><h2>Failure to apply the penalty to the provider because toxic offer</h2></body></html>";		
			}
		}
		else
		{
			print "<html><body><h2>Failure to apply the penalty to the provider because toxic offer</h2></body></html>";	
		}
    }	
?>
