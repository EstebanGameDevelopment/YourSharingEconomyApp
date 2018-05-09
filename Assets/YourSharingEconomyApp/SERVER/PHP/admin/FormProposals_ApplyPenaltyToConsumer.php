<?php
	
	include '../ConfigurationYourSharingEconomyApp.php';
    
	$id_consumer_reporter = $_POST["id"];
	$id_proposal_reported = $_POST["proposal"];

	PenaltyToTheConsumer($id_consumer_reporter, $id_proposal_reported);

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
     //  PenaltyToTheConsumer
     //-------------------------------------------------------------
     function PenaltyToTheConsumer($id_consumer_reporter_par, $id_proposal_reported_par)
     {
		$query_update_proposal = "UPDATE proposals SET confirmedreported = 1, reported = '' WHERE id = $id_proposal_reported_par";
		$result_update_proposal = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_proposal) or die("Query Error::RequestUpdateScore::Update users failed");
		 
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)		
		{
			$query_update_user_score_consumer = "UPDATE users SET votesuser = votesuser + 1 WHERE id = $id_consumer_reporter_par";
			$result_update_user_score_consumer = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user_score_consumer) or die("Query Error::RequestUpdateScore::Update users failed");

			if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)		
			{
				print "<html><body><h2>Success to apply the penalty to the consumer</h2></body></html>";
			}
			else
			{
				print "<html><body><h2>Failure to apply the penalty to the consumer because toxic offer</h2></body></html>";		
			}
		}
		else
		{
			print "<html><body><h2>Failure to apply the penalty to the consumer because toxic offer</h2></body></html>";	
		}
    }	
?>
