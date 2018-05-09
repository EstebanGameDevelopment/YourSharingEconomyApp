<?php
    
	include 'ConfigurationYourSharingEconomyApp.php';
    
	$idimage = $_GET["id"];
	
	// ++ INSERT LEVEL ++
	LoadImage($idimage);

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
     //  LoadImage
     //-------------------------------------------------------------
     function LoadImage($idimage_par)
     {
		$query_consult = "SELECT * FROM images WHERE id = $idimage_par";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::LoadImage::LoadImage");
		
		// Finalizacion de registro
		if ($row_consult = mysqli_fetch_object($result_consult))
		{
			$table_img = $row_consult->tabla;
			$idorigin_img = $row_consult->idorigin;
			$type_img = $row_consult->type;
			$size_img = $row_consult->size;
			$url_img = $row_consult->url;
			$data_img = $row_consult->data;
			
			print "true" . $GLOBALS['PARAM_SEPARATOR'] . $idimage_par . $GLOBALS['PARAM_SEPARATOR'] . $table_img . $GLOBALS['PARAM_SEPARATOR'] . $idorigin_img . $GLOBALS['PARAM_SEPARATOR'] . $type_img . $GLOBALS['PARAM_SEPARATOR'] . $url_img . $GLOBALS['PARAM_SEPARATOR'] . $size_img . $GLOBALS['PARAM_SEPARATOR'] . $data_img;
		}
		else
		{
			print "false";
		}
    }

?>
