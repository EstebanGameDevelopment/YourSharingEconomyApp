<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';
    
	$language = $_GET["language"];
	$iduser = $_GET["id"];
	$password = $_GET["password"];
	$request = $_GET["request"];

    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		ResetRequestAllProposals($language, $request);
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
     //  MailChangedRequest
     //-------------------------------------------------------------
	 function MailChangedRequest($language_par, $email_provider_par, $title_par)
	 {
		 if ($language_par == "en")
		 {
			 // EMAIL LEVEL
			 $to      = $email_provider_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Request changed: ' . $title_par;
			 $message = 'The customer has changed his request:' . $title_par . "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'Now you have to check out the changes and confirm again that you keep your offer'. "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'Thanks';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message);
		 }
		 if ($language_par == "es")
		 {
			 // EMAIL LEVEL
			 $to      = $email_provider_par;
			 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Encargo Modificado: ' . $title_par;
			 $message = 'El cliente ha cambiado el encargo:' . $title_par . "<p>";
			 $message = $message . "<p>";
			 $message = $message . 'Ahora tendrás que comprobar los cambios y confirmar de nuevo si quieres mantener tu oferta'. "<p>"; 
			 $message = $message . "<p>";
			 $message = $message . 'Gracias';
			 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
			 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
			 'X-Mailer: PHP/' . phpversion();

			 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message); 
		 }
	 }	
	 
     //-------------------------------------------------------------
     // ResetRequestAllProposals
     //-------------------------------------------------------------
     function ResetRequestAllProposals($language_par, $request_par)
     {
		// UPDATE PROPOSAL
		$query_update_proposal = "UPDATE proposals SET active = 0 WHERE request = $request_par AND type = 1";
		$result_update_proposal = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_proposal) or die("Query Error::ProposalsResetRequestAll::Update proposals failed");

		// Free resultset
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) > 0)
		{
			// SELECT ALL PROPOSAL AFFECTED
			$query_prop_consult = "SELECT * FROM proposals WHERE request = $request_par AND type = 1 AND active = 0";
			$result_prop_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_prop_consult) or die("Query Error::ProposalsResetRequestAll::Select proposals failed");

			$emailsToReport = array("none");
			while ($row_prop_consult = mysqli_fetch_object($result_prop_consult))
			{
				$id_prop = $row_prop_consult->id;
				$user_prop = $row_prop_consult->user;

				// CONSULT USERS
				$query_consult = "SELECT email FROM users WHERE id = $user_prop";
				$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::ProposalsResetRequestAll::Select email failed");

				// SEND EMAIL
				if ($row_user = mysqli_fetch_object($result_consult))
				{
					$email_user = DecryptText($row_user->email);
					$found_email = false;
					foreach ($emailsToReport as $valuemail) 
					{
						if ($valuemail === $email_user) 
						{
							$found_email = true;
						}
					}
					if ($found_email == false)
					{
						array_push($emailsToReport, $email_user);
					
						// CONSULT REQUEST
						$query_req_consult = "SELECT title FROM requests WHERE id = $request_par";
						$result_req_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_req_consult) or die("Query Error::ProposalsResetRequestAll::Select request title failed");

						if ($row_req_user = mysqli_fetch_object($result_req_consult))
						{
							MailChangedRequest($language_par, $email_user, $row_req_user->title);					
						}
					}
				}
				mysqli_free_result($result_consult);					
			}
			mysqli_free_result($result_prop_consult);
		}
		
		print "true";
    }
	
?>
