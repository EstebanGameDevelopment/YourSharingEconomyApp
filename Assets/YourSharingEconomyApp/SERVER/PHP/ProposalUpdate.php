<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$language = $_POST["language"];
	$iduser = $_POST["id"];
	$password = $_POST["password"];

	$proposal = $_POST["proposal"];
	$request = $_POST["request"];
	$price = $_POST["price"];
	$deadline = $_POST["deadline"];
	$provider = $_POST["provider"];
	$accepted = $_POST["accepted"];
	
    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		UpdateProposalUser($language,$email_db_user,$proposal,$request,$price,$deadline,$provider,$accepted);
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
     //  MailConnectionCustomer
     //-------------------------------------------------------------
	 function MailConnectionCustomer($language_par, $email_consumer_par, $email_provider_par)
	 {
		 if ($language_par == "en")
		 {
			 // EMAIL LEVEL
			 $to      = $email_consumer_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Deal Closed';
			 $message = 'You have closed the deal with a tailor'. "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'The email of the tailor is: ' . $email_provider_par . "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Now is up to you both to complete the job. Once completed the job, the tailor will upload a picture with the job done, then you both would be able to score each other behavior and professionality as a tailor and a customer'; 
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
			 $to      = $email_consumer_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Acuerdo cerrado';
			 $message = 'Has aceptado un acuerdo para tu encargo con un modista'. "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'El correo del modista es: ' . $email_provider_par . "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Ahora depende de vosotros completar el trabajo. Una vez completado el trabajo, el modista subira una fotografia con el trabajo acabado, solo entonces podreis puntuaros el comportamiento como consumidor y modista profesional'; 
			 $message = $message . "<p>";
			 $message = $message . 'Gracias';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message); 
		 }
	 }	

	 //-------------------------------------------------------------
     //  MailConnectionProvider
     //-------------------------------------------------------------
	 function MailConnectionProvider($language_par, $email_consumer_par, $email_provider_par)
	 {
		 if ($language_par == "en")
		 {
			 // EMAIL LEVEL
			 $to      = $email_provider_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Deal Closed';
			 $message = 'A customer has finally closed you offer'. "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'The email of the customer is: ' . $email_consumer_par . "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Now is up to you both to complete the job. Once completed the job, the tailor will upload a picture with the job done, then you both would be able to score each other behavior and professionality as a tailor and a customer'; 
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
			 $to      = $email_provider_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Acuerdo cerrado';
			 $message = 'Un cliente ha aceptado tu oferta'. "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'El correo del cliente es: ' . $email_consumer_par . "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Ahora depende de vosotros completar el trabajo. Una vez completado el trabajo, el modista subira una fotografia con el trabajo acabado, solo entonces podreis puntuaros el comportamiento como consumidor y modista profesional'; 
			 $message = $message . "<p>";
			 $message = $message . 'Gracias';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message); 
		 }
	 }	

     //-------------------------------------------------------------
     //  UpdateProposalUser
     //-------------------------------------------------------------
     function UpdateProposalUser($language_par, $email_consumer_par, $proposal_par, $request_par, $price_par, $deadline_par, $provider_par, $accepted_par)
     {
		// UPDATE PROPOSAL
		$query_update_proposal = "UPDATE proposals SET accepted = $accepted_par WHERE id = $proposal_par";
		$result_update_proposal = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_proposal) or die("Query Error::UpdateProposalUser::Update proposals failed");

		// Free resultset
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			$final_provider_id = -1;
			if ($accepted_par == 2)
			{
				$final_provider_id = $provider_par;
			}
			
			// UPDATE REQUEST WITH NEW DATA
			$query_update_requests = "UPDATE requests SET price = $price_par, deadline=$deadline_par, provider=$final_provider_id WHERE id = $request_par";
			$result_update_requests = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_requests) or die("Query Error::UpdateProposalUser::Update requests failed");
			
			if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
			{
				// UPDATE PROPOSAL
				$query_reset_proposal = "UPDATE proposals SET accepted = -1 WHERE request = $request_par AND id <> $proposal_par";
				$result_reset_proposal = mysqli_query($GLOBALS['LINK_DATABASE'],$query_reset_proposal) or die("Query Error::UpdateProposalUser::Update proposals part 2 failed");
				
				if ($accepted_par == 2)
				{
					$query_consult = "SELECT * FROM users WHERE id = $provider_par";
					$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::UpdateProposalUser::Select users failed");

					if ($row_user = mysqli_fetch_object($result_consult))
					{
						$email_provider = DecryptText($row_user->email);
									
						MailConnectionCustomer($language_par, $email_consumer_par, $email_provider);
						MailConnectionProvider($language_par, $email_consumer_par, $email_provider);
					}
					mysqli_free_result($result_consult);
				}
				print "true";	
			}
			else
			{
				print "false";
			}
		}
		else
		{
			print "false";
		}
    }

	
?>
