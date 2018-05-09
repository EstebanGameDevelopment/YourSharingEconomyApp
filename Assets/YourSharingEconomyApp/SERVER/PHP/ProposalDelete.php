<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$proposal = $_GET["proposal"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		DeleteProposal($proposal);
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
     //  DeleteProposal
     //-------------------------------------------------------------
     function DeleteProposal($proposal_par)
     {
		// ++ GET MAX ID ++
		$query_delete = "DELETE FROM proposals WHERE id = $proposal_par";
		$result_delete = mysqli_query($GLOBALS['LINK_DATABASE'],$query_delete) or die("Query Error::ProposalDelete::DeleteProposal");

		// Finalizacion de registro
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			print "true";
		}
		else
		{
			print "false";
		}
    }
	
?>
