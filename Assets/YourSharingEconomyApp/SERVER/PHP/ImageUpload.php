<?php
	include 'ConfigurationYourSharingEconomyApp.php';

	$id_img = $_POST["id"];
	$table_origin = $_POST["table"];
	$id_origin = $_POST["idorigin"];
	$type_img = $_POST["type"];
	$data_size = $_POST["size"];
	$url_data = $_POST["url"];
	$data_temporal = $_FILES["data"];
	$data_img = file_get_contents($data_temporal['tmp_name']);
	
	$iduser = $_POST["user"];
	$password = $_POST["password"];
	
    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		UploadImage($table_origin, $id_origin, $data_size, $data_img, $type_img, $url_data);
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
     function UploadImage($table_origin_par, $id_origin_par, $data_size_par, $data_img_par, $type_img_par, $url_data_par)
     {
		// ++ GET MAX ID ++
		$query_consult = "SELECT max(id) as maximumId FROM images";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::ImageUpload::UploadImage::Get max id");
		$row_consult = mysqli_fetch_object($result_consult);
		$maxIdentifier = $row_consult->maximumId;
		$maxIdentifier = $maxIdentifier + 1;
		mysqli_free_result($result_consult);
		
		$timestamp_local_calculated = time();
		$query_insert = "INSERT INTO images VALUES ($maxIdentifier, '$table_origin_par', $id_origin_par, $data_size_par,'".mysqli_real_escape_string($GLOBALS['LINK_DATABASE'],$data_img_par)."', $timestamp_local_calculated, $type_img_par, '$url_data_par')";
		$result_insert = mysqli_query($GLOBALS['LINK_DATABASE'],$query_insert) or die("Query Error::ImageUpload::UploadImage::Insert new image failed");
		
		// Finalizacion de registro
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			print "true" . $GLOBALS['PARAM_SEPARATOR'] . $maxIdentifier . $GLOBALS['PARAM_SEPARATOR'] . $table_origin_par . $GLOBALS['PARAM_SEPARATOR'] . $id_origin_par;
		}
		else
		{
			print "false";
		}
    }

?>
