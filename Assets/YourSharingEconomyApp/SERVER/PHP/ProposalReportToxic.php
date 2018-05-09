<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$proposal = $_GET["proposal"];
	$reporter = $_GET["reporter"];
	$request = $_GET["request"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ReportToxicProposal($proposal, $reporter, $request);
	}
	else
	{
		print "false";
	}

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
     // ReportToxicProposal
     //-------------------------------------------------------------
     function ReportToxicProposal($proposal_par, $reporter_par, $request_par)
     {
		// UPDATE PROPOSAL
		$query_update_proposal = "UPDATE proposals SET reported = '$reporter_par' WHERE id = $proposal_par";
		$result_update_proposal = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_proposal) or die("Query Error::ProposalToxicReport::ReactivateProposal");
		
		// IF CHANGES
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			print "true" . $GLOBALS['PARAM_SEPARATOR'] . $proposal_par;
		}
		else
		{
			print "false";
		}
    }
	
?>
