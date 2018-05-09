<?php
	include 'ConfigurationYourSharingEconomyApp.php';
	
	$idimage = $_GET["id"];
	$iduser = $_GET["user"];
	$password = $_GET["password"];
	
    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		RemoveImage($idimage);
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
     //  UploadImage
     //-------------------------------------------------------------
     function RemoveImage($idimage_par)
     {
		$query_insert = "DELETE FROM images WHERE id = $idimage_par";
		$result_insert = mysqli_query($GLOBALS['LINK_DATABASE'],$query_insert) or die("Query Error::ImageRemove::RemoveImage");
		
		// Finalizacion de registro
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			print "true" . $GLOBALS['PARAM_SEPARATOR'] . $idimage_par;
		}
		else
		{
			print "false";
		}
    }

?>
