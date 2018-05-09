<?php
	
	include 'ConfigurationYourSharingEconomyApp.php';

	$languageuser = $_POST["language"];
	$iduser = $_POST["id"];
	$password = $_POST["password"];

	$proposal_user = $_POST["proposal"];
	$user_id = $_POST["user"];
	$request_user = $_POST["request"];
	$type_user = $_POST["type"];	
	$title_user = $_POST["title"];
	$description_user = $_POST["description"];
	$price_user = $_POST["price"];
	$deadline_user = $_POST["deadline"];
	$accepted_user = $_POST["accepted"];
	
    $email_db_user = ExistsUser($iduser, $password);
	if (strlen($email_db_user) > 0)
	{
		if (CheckLastProposal($user_id,$request_user))
		{
			CreateProposal($languageuser, $email_db_user, $proposal_user, $user_id, $request_user, $type_user, $title_user, $description_user, $price_user, $deadline_user, $accepted_user);
		}
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
     //  MailNewOffer
     //-------------------------------------------------------------
	 function MailNewOffer($language_par, $email_real_par, $send_type_email_to_customer_par)
	 {
		 $message = "";
		 switch ($send_type_email_to_customer_par)
		 {
			case 0:			 
				 if ($language_par == "en")
				 {
					 // EMAIL LEVEL
					 $to      = $email_real_par;
					 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' New Offer';
					 $message = $message . "<p>";
					 $message = 'Your tailor is interested in your request.'. "<p>";
					 $message = $message . "<p>";
					 $message = $message . 'Please, check her offer in order to consider it' . "<p>"; 
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
					 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Nueva Oferta';
					 $message = $message . "<p>";
					 $message = 'Un modista se ha interesado por tu encargo'. "<p>"; 
					 $message = $message . "<p>";
					 $message = $message . 'Por favor, comprueba si oferta para ver si se ajusta a tus necesidades' . "<p>"; 
					 $message = $message . "<p>";
					 $message = $message . "<p>";
					 $message = $message . 'Gracias';
					 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
					 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
					 'X-Mailer: PHP/' . phpversion();

					 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message); 
				 }
				 break;
				 
			case 1:			 
				 if ($language_par == "en")
				 {
					 // EMAIL LEVEL
					 $to      = $email_real_par;
					 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Your request have been reported';
					 $message = $message . "<p>";
					 $message = 'Your request has been reported. Please, check your request and make all the necesary changes to have a valid request.'. "<p>";
					 $message = $message . "<p>";
					 $message = $message . 'If you fail to make the changes and keep an offensive request this activity will be reflected forever in your account' . "<p>"; 
					 $message = $message . "<p>";
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
					 $to      = $email_real_par;
					 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Tu encargo ha sido denunciado';
					 $message = $message . "<p>";
					 $message = 'Tu encargo ha sido denunciado. Por favor, comprueba que tu encargo sea correcto y haga todos los cambios necesareos para que este sea válido.'. "<p>"; 
					 $message = $message . "<p>";
					 $message = $message . 'Si no realiza los cambios y mantiene un encargo ofensivo esta actividad se vera reflejada para siempre en su cuenta' . "<p>"; 
					 $message = $message . "<p>";
					 $message = $message . "<p>";
					 $message = $message . 'Gracias';
					 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
					 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
					 'X-Mailer: PHP/' . phpversion();

					 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message); 
				 }
				 break;
				 
			case 2:			 
				 if ($language_par == "en")
				 {
					 // EMAIL LEVEL
					 $to      = $email_real_par;
					 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Your request has been flagged as innappropiate';
					 $message = $message . "<p>";
					 $message = 'We are sorry to inform you that your request has been flagged as innappropiate.'. "<p>";
					 $message = $message . "<p>";
					 $message = $message . 'This activity will be reflected in your account in order for the people to know you.' . "<p>"; 
					 $message = $message . "<p>";
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
					 $to      = $email_real_par;
					 $subject = $GLOBALS['OFFICIAL_NAME_APPLICATION_GLOBAL'] . ' Tu encargo ha sido marcado como inapropiado';
					 $message = $message . "<p>";
					 $message = 'Sentimos informarle que su encargo ha sido marcado como inapropiado.'. "<p>"; 
					 $message = $message . "<p>";
					 $message = $message . 'Esta actividad sera reflejada en su cuenta para que la gente le pueda conocer' . "<p>"; 
					 $message = $message . "<p>";
					 $message = $message . "<p>";
					 $message = $message . 'Gracias';
					 $headers = 'From: no-reply@yourdressmakers.com' . "<p>" .
					 'Reply-To: no-reply@yourdressmakers.com' . "<p>" .
					 'X-Mailer: PHP/' . phpversion();

					 SendGlobalEmail($GLOBALS['NON_REPLY_EMAIL_ADDRESS'], $to, $subject, $message); 
				 }
				 break;				 
		 }
	 }		 
	 
     //-------------------------------------------------------------
     //  CheckLastProposal
     //-------------------------------------------------------------
     function CheckLastProposal($user_par, $request_par)
     {
		// CHECK IF PREMIUM POST OFFER
		$query_usr_consult = "SELECT additionaloffer FROM users WHERE id = $user_par";
		$result_usr_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_usr_consult) or die("Query Error::ProposalCreate::CheckLastProposal::Select additional offer failed");

		if ($row_user = mysqli_fetch_object($result_usr_consult))
		{
			$additionaloffer_user = $row_user->additionaloffer;
			if ($additionaloffer_user > 0)
			{
				return true;
			}
		}
		 
		// CHECK LAST TIME POSTED OFFER
		$query_consult = "SELECT max(created) as maximumcreated FROM proposals WHERE user=$user_par AND request=$request_par";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::ProposalCreate::CheckLastProposal::Select max");
		
		if ($row_consult = mysqli_fetch_object($result_consult))
		{
			$created_pro = $row_consult->maximumcreated;
			$timestamp_local_calculated = time();
			
			$timediference = $timestamp_local_calculated - $created_pro;			
			$hoursdiference = (($timediference/60)/60);
			
			if ($hoursdiference < $GLOBALS['HOURS_TO_WAIT_FOR_NEW_PROPOSAL'])
			{
				print "false" . $GLOBALS['PARAM_SEPARATOR'] . $hoursdiference;	
				return false;
			}
			else
			{
				return true;
			}
		}
		return true;
	 }

     //-------------------------------------------------------------
     //  CreateRequest
     //-------------------------------------------------------------
     function CreateProposal($languageuser_par, $email_db_par, $proposal_par, $user_par, $request_par, $type_par, $title_par, $description_par, $price_par, $deadline_par, $accepted_par)
     {
		// GET MAX ID
		$query_consult = "SELECT max(id) as maximumId FROM proposals";
		$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::ProposalCreate::CreateProposal::Select max");
		$row_consult = mysqli_fetch_object($result_consult);
		$maxIdentifier = $row_consult->maximumId;
		mysqli_free_result($result_consult);

		$proposal_id_new = $maxIdentifier + 1;
		$timestamp_local_calculated = time();

		$title_par_f = SpecialCharacters($title_par);
		$description_par_f = SpecialCharacters($description_par);
		
		// INSERT NEW PROPOSAL
		$query_insert = "INSERT INTO proposals VALUES ($proposal_id_new, $user_par, $request_par,	$type_par, '$title_par_f', '$description_par_f', $price_par, $deadline_par, -1, $timestamp_local_calculated, 1, '', 0)";
		$result_insert = mysqli_query($GLOBALS['LINK_DATABASE'],$query_insert) or die("Query Error::ProposalCreate::CreateProposal::Insert proposal failed");
		
		$users_who_reported = "";
		
		// Free resultset
		if (mysqli_affected_rows($GLOBALS['LINK_DATABASE']) == 1)
		{
			// UPDATE PREMIUM POST
			$query_update_user = "UPDATE users SET additionaloffer = 0 WHERE id = $user_par";
			$result_update_user = mysqli_query($GLOBALS['LINK_DATABASE'],$query_update_user) or die("Query Error::ProposalCreate::CreateProposal::Update user failed");
			
			// IF PROPOSAL IS TYPE_REPORT THEN A DIFFERENT ROUTE
			$send_type_email_to_customer = -1;
			if ($type_par == 2)
			{
				// GET EMAIL OF CUSTOMER AND SEND HIM AN EMAIL
				$query_report_consult = "SELECT reported FROM requests WHERE id = $request_par";
				$result_report_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_report_consult) or die("Query Error::ProposalCreate::CreateProposal::Select report failed");

				if ($row_report_consult = mysqli_fetch_object($result_report_consult))
				{
					$users_who_reported = $row_report_consult->reported;
					$user_current_str = "" . $user_par;
					
					if (!stripos($users_who_reported,$user_current_str))
					{
						if (strlen($users_who_reported) == 0)
						{
							$users_who_reported	= $user_current_str;
						}
						else
						{
							$users_who_reported	= $users_who_reported . "," . $user_current_str;
						}
					}
					else
					{
						$users_who_reported ="";	
					}
					
					if (strlen($users_who_reported) > 0)
					{
						// COUNT THE NUMBER OF USERS WHO HAD REPORTED TO FLAG THE POST
						$array_reporters = explode(',',$users_who_reported);
						
						// IF REPORTED 4 TIMES OR MORE THEN FLAG THE REQUEST						
						$final_flag = 0;
						if (sizeof($array_reporters) > $GLOBALS['TOTAL_REPORT_REQUEST_TO_PENALTY'])
						{
							$final_flag = 1;
							$send_type_email_to_customer = 2;
						}
						else
						{
							$send_type_email_to_customer = 1;
						}
						
						$query_report_update = "UPDATE requests SET reported = '$users_who_reported', flaged = $final_flag  WHERE id = $request_par";
						$result_report_update = mysqli_query($GLOBALS['LINK_DATABASE'],$query_report_update) or die("Query Error::ProposalCreate::CreateProposal::Update report failed");
					}
				}
			}
			
			// GET NOTIFICATIONS
			$query_customer_consult = "SELECT customer,notifications FROM requests WHERE id = $request_par";
			$result_customer_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_customer_consult) or die("Query Error::ProposalCreate::CreateProposal::Select customer failed");
			
			if ($row_customer_consult = mysqli_fetch_object($result_customer_consult))
			{
				$id_customer = $row_customer_consult->customer;
				$notifications_customer = $row_customer_consult->notifications;	

				// IF NOTIFICATIONS ACTIVATED SEND EMAIL WITH THE OFFER
				if ($send_type_email_to_customer ==  -1)
				{
					if ($notifications_customer == 1)
					{
						$send_type_email_to_customer = 0;
					}
				}
				
				// SEND EMAIL CUSTOMER
				if ($send_type_email_to_customer != -1)
				{
					$query_email_consult = "SELECT email FROM users WHERE id = $id_customer";
					$result_email_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_email_consult) or die("Query Error::ProposalCreate::CreateProposal::Select email failed");

					if ($row_email_consult = mysqli_fetch_object($result_email_consult))
					{
						$email_real_customer = DecryptText($row_email_consult->email);						
						MailNewOffer($languageuser_par, $email_real_customer, $send_type_email_to_customer);
					}
				}			
			}
			
			print "true" . $GLOBALS['PARAM_SEPARATOR'] . $proposal_id_new . $GLOBALS['PARAM_SEPARATOR'] . $type_par . $GLOBALS['PARAM_SEPARATOR'] . $request_par . $GLOBALS['PARAM_SEPARATOR'] . $users_who_reported;
		}
		else
		{
			print "false";
		}
    }

	
?>
