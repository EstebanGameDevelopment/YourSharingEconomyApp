<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$language_user = $_GET["language"];
	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$requestid = $_GET["request"];
	$broken = $_GET["broken"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{	
		UpdateRequestAsFinished($language_user, $requestid, $broken);
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
     //  MailPassword
     //-------------------------------------------------------------
	 function MailPassword($language_par, $email_real_par)
	 {
		 $message = "";
		 if ($language_par == "en")
		 {
			 // EMAIL LEVEL
			 $to      = $email_real_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Job completed';
			 $message = $message . "<p>";
			 $message = 'Your tailor has completed successfully your request.'. "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'Please, check the images uploaded by the tailor and give and score to the job in order to approve it' . "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'Best';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message);
		 }
		 if ($language_par == "es")
		 {
			 // EMAIL LEVEL
			 $to      = $email_real_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Encargo completado';
			 $message = $message . "<p>";
			 $message = 'Tu modista ha acabdo el trabajo exitosamente'. "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Por favor, comprueba las imagenes subidas por el modista y dale una puntuacion para aprovar su trabajo' . "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'Gracias';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message); 
		 }
	 }		 
	
	 //-------------------------------------------------------------
     //  UpdateRequestAsFinished
     //-------------------------------------------------------------
     function UpdateRequestAsFinished($language_par, $requestid_par, $broken_par)
     {
		$finisheddate_in_server = time();
		if ($broken_par == 1) $finisheddate_in_server = 0;		
		$query_update_request = "UPDATE requests SET deliverydate=$finisheddate_in_server WHERE id = $requestid_par";
		$result_update_request = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_request) or die("Query Error::RequestUpdateAsFinished::Update requests failed");

		// Free resultset
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			// GET EMAIL OF CUSTOMER AND SEND HIM AN EMAIL
			$query_customer_consult = "SELECT customer FROM requests WHERE id = $requestid_par";
			$result_customer_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_customer_consult) or die("Query Error::RequestUpdateAsFinished::Select customer failed");

			if ($row_customer_consult = mysqli_fetch_object($result_customer_consult))
			{
				$id_customer = $row_customer_consult->customer;

				// GET EMAIL CUSTOMER
				$query_email_consult = "SELECT email FROM users WHERE id = $id_customer";
				$result_email_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_email_consult) or die("Query Error::RequestUpdateAsFinished::Select email failed");

				if ($row_email_consult = mysqli_fetch_object($result_email_consult))
				{
					$email_real_customer = DecryptText($row_email_consult->email);	
					
					// SEND EMAIL REPORTING JOB FINISHED
					MailPassword($language_par, $email_real_customer);
				}
			}
			
			print "true" . $GLOBALS['PARAM_SEPARATOR'] . $requestid_par;
		}
		else
		{
			print "false";
		}
	 }

	
?>
