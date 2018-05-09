<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$origin = $_GET["origin"];
	$tabla = $_GET["tabla"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ConsultAllImages($origin, $tabla);
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
     //  ConsultAllImages
     //-------------------------------------------------------------
     function ConsultAllImages($origin_par, $tabla_par)
     {
		// GET LINKED IMAGES
		$query_imgs_consult = "SELECT id,size,type,url FROM images WHERE idorigin = $origin_par AND tabla='$tabla_par'";
		$result_imgs_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_imgs_consult) or die("Query Error::ImagesConsultAll::ConsultAllImages");
	
		$output_packet = "";
		while ($row_imgs_consult = mysqli_fetch_object($result_imgs_consult))		
		{
			$line_img = $row_imgs_consult->id . $GLOBALS['PARAM_SEPARATOR'] . $tabla_par . $GLOBALS['PARAM_SEPARATOR'] . $origin_par . $GLOBALS['PARAM_SEPARATOR'] . $row_imgs_consult->size . $GLOBALS['PARAM_SEPARATOR'] . $row_imgs_consult->type . $GLOBALS['PARAM_SEPARATOR'] . $row_imgs_consult->url;
			
			$output_packet = $output_packet . $GLOBALS['LINE_SEPARATOR'] . $line_img;
		}
		
		print $output_packet;		
		
		mysqli_free_result($result_imgs_consult);
    }
	
?>
