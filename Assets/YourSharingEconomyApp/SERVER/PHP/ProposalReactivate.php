<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$proposal = $_GET["proposal"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ReactivateProposal($proposal);
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
     // ReactivateProposal
     //-------------------------------------------------------------
     function ReactivateProposal($proposal_par)
     {
		// UPDATE PROPOSAL
		$query_update_proposal = "UPDATE proposals SET active = 1 WHERE id = $proposal_par";
		$result_update_proposal = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_proposal) or die("Query Error::ProposalReactivate::ReactivateProposal");
		
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
