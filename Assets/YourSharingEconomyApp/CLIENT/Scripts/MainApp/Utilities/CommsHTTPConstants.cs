using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourCommonTools;

namespace YourSharingEconomyApp
{
	public class CommsHTTPConstants
	{
		// ----------------------------------------------
		// COMM EVENTS
		// ----------------------------------------------	
		public const string EVENT_COMM_CONFIGURATION_PARAMETERS = "YourSharingEconomyApp.GetConfigurationServerParametersHTTP";
		public const string EVENT_COMM_REQUEST_USER_BY_FACEBOOK = "YourSharingEconomyApp.UserLoginByFacebookHTTP";
		public const string EVENT_COMM_REQUEST_USER_BY_LOGIN = "YourSharingEconomyApp.UserLoginByEmailHTTP";
		public const string EVENT_COMM_REQUEST_USER_REGISTER = "YourSharingEconomyApp.UserRegisterWithEmailHTTP";
		public const string EVENT_COMM_REQUEST_USER_CONSULT = "YourSharingEconomyApp.UserConsultByIdHTTP";
		public const string EVENT_COMM_REQUEST_UPDATE_PROFILE = "YourSharingEconomyApp.UserUpdateProfileHTTP";
		public const string EVENT_COMM_REQUEST_RESET_PASSWORD = "YourSharingEconomyApp.UserRequestResetPasswordHTTP";
		public const string EVENT_COMM_REQUEST_RESET_BY_EMAIL_PASSWORD = "YourSharingEconomyApp.UserRequestResetByEmailPasswordHTTP";
		public const string EVENT_COMM_CHECK_VALIDATION_USER = "YourSharingEconomyApp.UserCheckValidationUserHTTP";
		public const string EVENT_COMM_CREATE_REQUEST_NEW_JOB = "YourSharingEconomyApp.RequestCreateNewJobHTTP";
		public const string EVENT_COMM_UPDATE_REQUEST_IMG_REFERENCE = "YourSharingEconomyApp.RequestUpdateImageRefHTTP";
		public const string EVENT_COMM_CONSULT_REQUESTS_BY_USER = "YourSharingEconomyApp.RequestConsultByUserHTTP";
		public const string EVENT_COMM_CONSULT_REQUESTS_BY_PROVIDER = "YourSharingEconomyApp.RequestConsultByProviderHTTP";
		public const string EVENT_COMM_CONSULT_REQUESTS_BY_DISTANCE = "YourSharingEconomyApp.RequestConsultByDistanceHTTP";
		public const string EVENT_COMM_CONSULT_SINGLE_REQUEST = "YourSharingEconomyApp.RequestSingleConsultHTTP";
		public const string EVENT_COMM_CONSULT_DELETE_REQUEST = "YourSharingEconomyApp.RequestDeleteRecordHTTP";
		public const string EVENT_COMM_SET_REQUEST_AS_FINISHED = "YourSharingEconomyApp.RequestSetAsFinishedHTTP";
		public const string EVENT_COMM_UPDATE_REQUEST_SCORE_AND_FEEDBACK = "YourSharingEconomyApp.RequestUpdateScoreAndFeedbackHTTP";
		public const string EVENT_COMM_UPLOAD_IMAGE = "YourSharingEconomyApp.ImageUploadHTTP";
		public const string EVENT_COMM_REMOVE_IMAGE = "YourSharingEconomyApp.ImageRemoveHTTP";
		public const string EVENT_COMM_LOAD_IMAGE = "YourSharingEconomyApp.ImageLoadHTTP";
		public const string EVENT_COMM_CONSULT_ALL_IMAGE_OF_REQUEST = "YourSharingEconomyApp.ImageConsultAllHTTP";
		public const string EVENT_COMM_CREATE_NEW_PROPOSAL = "YourSharingEconomyApp.ProposalCreateNewHTTP";
		public const string EVENT_COMM_CONSULT_ALL_PROPOSALS_OF_REQUEST = "YourSharingEconomyApp.ProposalConsultAllHTTP";
		public const string EVENT_COMM_RESET_ALL_PROPOSALS_OF_REQUEST = "YourSharingEconomyApp.ProposalResetAllHTTP";
		public const string EVENT_COMM_REACTIVATE_PROPOSAL = "YourSharingEconomyApp.ProposalReactivateAllHTTP";
		public const string EVENT_COMM_REPORT_PROPOSAL = "YourSharingEconomyApp.ProposalReportToxicHTTP";
		public const string EVENT_COMM_UPDATE_PROPOSAL = "YourSharingEconomyApp.ProposalUpdateHTTP";
		public const string EVENT_COMM_REMOVE_PROPOSAL = "YourSharingEconomyApp.ProposalRemoveHTTP";
		public const string EVENT_COMM_IAP_CODE_REGISTER = "YourSharingEconomyApp.IAPCreateValidationCodeHTTP";
		public const string EVENT_COMM_IAP_RENT_TIME_PROVIDER = "YourSharingEconomyApp.IAPRentTimeProviderHTTP";
		public const string EVENT_COMM_IAP_PREMIUM_OFFER = "YourSharingEconomyApp.IAPPremiumPostOfferHTTP";
		public const string EVENT_COMM_IAP_PREMIUM_REQUEST = "YourSharingEconomyApp.IAPPremiumRequestHTTP";

		public const string EVENT_COMM_BITCOIN_REGISTER_TRANSACTION = "YourSharingEconomyApp.RequestUpdateTransactionHTTP";

		// -------------------------------------------
		/* 
		 * DisplayLog
		 */
		public static void DisplayLog(string _data)
		{
			CommController.Instance.DisplayLog(_data);
		}

		// -------------------------------------------
		/* 
		 * GetServerConfigurationParameters
		 */
		public static void GetServerConfigurationParameters()
		{
			CommController.Instance.Request(EVENT_COMM_CONFIGURATION_PARAMETERS, true);
		}

		// -------------------------------------------
		/* 
		 * RequestUserByLogin
		 */
		public static void RequestUserByLogin(string _email, string _password)
		{
			CommController.Instance.Request(EVENT_COMM_REQUEST_USER_BY_LOGIN, false, _email, _password);
		}

		// -------------------------------------------
		/* 
		 * RequestUserRegister
		 */
		public static void RequestUserRegister(string _email, string _password)
		{
			CommController.Instance.Request(EVENT_COMM_REQUEST_USER_REGISTER, false, _email, _password);
		}

		// -------------------------------------------
		/* 
		 * RequestUserRegister
		 */
		public static void RequestConsultUser(long _idOwnUser, string _password, long _idUserSearch)
		{
			CommController.Instance.Request(EVENT_COMM_REQUEST_USER_CONSULT, false, _idOwnUser.ToString(), _password, _idUserSearch.ToString());
		}

		// -------------------------------------------
		/* 
		 * RequestUserByFacebook
		 */
		public static void RequestUserByFacebook(string _idFacebook, string _nameFacebook, string _emailFacebook, string _friendsPackage)
		{
			CommController.Instance.Request(EVENT_COMM_REQUEST_USER_BY_FACEBOOK, false, _idFacebook, _nameFacebook, _emailFacebook, _friendsPackage);
		}

		// -------------------------------------------
		/* 
		 * RequestUpdateProfile
		 */
		public static void RequestUpdateProfile(string _idUser, string _currentPassword, string _password, string _email, string _name, string _village, string _mapdata, string _skills, string _description, string _publicKeyAddress)
		{
			CommController.Instance.Request(EVENT_COMM_REQUEST_UPDATE_PROFILE, true, _idUser, _currentPassword, _password, _email, _name, _village, _mapdata, _skills, _description, _publicKeyAddress);
		}

		// -------------------------------------------
		/* 
		 * RequestUpdateProfile
		 */
		public static void RequestResetPassword(string _idUser)
		{
			CommController.Instance.Request(EVENT_COMM_REQUEST_RESET_PASSWORD, true, _idUser);
		}

		// -------------------------------------------
		/* 
		 * RequestUpdateProfile
		 */
		public static void RequestResetPasswordByEmail(string _email)
		{
			CommController.Instance.Request(EVENT_COMM_REQUEST_RESET_BY_EMAIL_PASSWORD, true, _email);
		}

		// -------------------------------------------
		/* 
		 * CheckValidationUser
		 */
		public static void CheckValidationUser(string _idUser)
		{
			CommController.Instance.Request(EVENT_COMM_CHECK_VALIDATION_USER, true, _idUser);
		}

		// -------------------------------------------
		/* 
		 * CheckValidationUser
		 */
		public static void CreateNewRequestDress(string _idUser, string _password, RequestModel _requestDress)
		{
			CommController.Instance.Request(EVENT_COMM_CREATE_REQUEST_NEW_JOB, true, _idUser, _password,
											_requestDress.Id.ToString(),
											_requestDress.Customer.ToString(),
											_requestDress.Provider.ToString(),
											_requestDress.Title,
											_requestDress.Description,
											_requestDress.Images.ToString(),
											_requestDress.Referenceimg.ToString(),
											_requestDress.Village,
											_requestDress.Mapdata,
											_requestDress.Latitude.ToString(),
											_requestDress.Longitude.ToString(),
											_requestDress.Price.ToString(),
											_requestDress.Currency,
											_requestDress.Distance.ToString(),
											_requestDress.GetFlags(),
											_requestDress.Notifications.ToString(),
											_requestDress.Creationdate.ToString(),
											_requestDress.Deadline.ToString(),
											_requestDress.FeedbackCustomerGivesToTheProvider,
											_requestDress.ScoreCustomerGivesToTheProvider.ToString(),
											_requestDress.FeedbackProviderGivesToTheCustomer,
											_requestDress.ScoreProviderGivesToTheCustomer.ToString()
				);
		}

		// -------------------------------------------
		/* 
		 * CreateNewRequestDress
		 */
		public static void UpdateImageReferenceRequest(int _idUser, string _password, long _idRequest, long _imageReference)
		{
			CommController.Instance.Request(EVENT_COMM_UPDATE_REQUEST_IMG_REFERENCE, true, _idUser.ToString(), _password, _idRequest.ToString(), _imageReference.ToString());
		}

		// -------------------------------------------
		/* 
		 * RequestConsultRecordsByUser
		 */
		public static void RequestConsultRecordsByUser(int _id, string _password, int _idUser)
		{
			CommController.Instance.RequestPriority(EVENT_COMM_CONSULT_REQUESTS_BY_USER, false, _id.ToString(), _password, _idUser.ToString());
		}

		// -------------------------------------------
		/* 
		 * RequestConsultRecordsByProvider
		 */
		public static void RequestConsultRecordsByProvider(int _id, string _password, int _idProvider, bool _allRecords)
		{
			CommController.Instance.RequestPriority(EVENT_COMM_CONSULT_REQUESTS_BY_PROVIDER, false, _id.ToString(), _password, _idProvider.ToString(), (_allRecords ? "1" : "0"));
		}

		// -------------------------------------------
		/* 
		 * RequestConsultRecordsByUser
		 */
		public static void RequestConsultRecordsByDistance(int _id, string _password, SearchModel _searchRequest, int _stateWorks)
		{
			CommController.Instance.RequestPriority(EVENT_COMM_CONSULT_REQUESTS_BY_DISTANCE, false, _id.ToString(), _password, _searchRequest.Latitude.ToString(), _searchRequest.Longitude.ToString(), _searchRequest.Distance.ToString(), _stateWorks.ToString());
		}

		// -------------------------------------------
		/* 
		 * RequestConsultRecordsByUser
		 */
		public static void RequestConsultSingleRequest(int _id, string _password, long _request)
		{
			CommController.Instance.RequestPriority(EVENT_COMM_CONSULT_SINGLE_REQUEST, false, _id.ToString(), _password, _request.ToString());
		}

		// -------------------------------------------
		/* 
		 * DeleteRequest
		 */
		public static void DeleteRequest(int _id, string _password, long _requestID)
		{
			CommController.Instance.RequestPriority(EVENT_COMM_CONSULT_DELETE_REQUEST, true, _id.ToString(), _password, _requestID.ToString());
		}

		// -------------------------------------------
		/* 
		 * SetRequestAsFinished
		 */
		public static void SetRequestAsFinished(int _id, string _password, long _requestID, bool _broken)
		{
			CommController.Instance.RequestPriority(EVENT_COMM_SET_REQUEST_AS_FINISHED, true, _id.ToString(), _password, _requestID.ToString(), (_broken ? "1" : "0"));
		}

		// -------------------------------------------
		/* 
		 * UpdateRequestScoreAndFeedback
		 */
		public static void UpdateRequestScoreAndFeedback(int _id, string _password, long _requestID, int _scoreConsumer, string _feedbackConsumer, int _scoreProvider, string _feedbackProvider, string _signatureCustomer, string _signatureProvider)
		{
			CommController.Instance.RequestPriority(EVENT_COMM_UPDATE_REQUEST_SCORE_AND_FEEDBACK, true, _id.ToString(), _password, _requestID.ToString(), _scoreConsumer.ToString(), _feedbackConsumer, _scoreProvider.ToString(), _feedbackProvider, _signatureCustomer, _signatureProvider);
		}

		// -------------------------------------------
		/* 
		 * UploadImage
		 */
		public static void UploadImage(long _idImage, string _table, long _idOriginTable, int _type, byte[] _data, string _url)
		{
			CommController.Instance.Request(EVENT_COMM_UPLOAD_IMAGE, true, _idImage.ToString(), _table, _idOriginTable.ToString(), _type.ToString(), _url, _data);
		}

		// -------------------------------------------
		/* 
		 * RemoveImage
		 */
		public static void RemoveImage(long _idImage)
		{
			CommController.Instance.Request(EVENT_COMM_REMOVE_IMAGE, true, _idImage.ToString());
		}

		// -------------------------------------------
		/* 
		 * ConsultImagesRequest
		 */
		public static void ConsultImagesRequest(int _id, string _password, long _idOrigin, string _table)
		{
			CommController.Instance.Request(EVENT_COMM_CONSULT_ALL_IMAGE_OF_REQUEST, true, _id.ToString(), _password, _idOrigin.ToString(), _table);
		}


		// -------------------------------------------
		/* 
		 * CreateNewProposal
		 */
		public static void CreateNewProposal(string _idUser, string _password, ProposalModel _proposal)
		{
			CommController.Instance.Request(EVENT_COMM_CREATE_NEW_PROPOSAL, true, _idUser, _password,
											_proposal.Id.ToString(),
											_proposal.User.ToString(),
											_proposal.Request.ToString(),
											_proposal.Type.ToString(),
											_proposal.Title,
											_proposal.Description,
											_proposal.Price.ToString(),
											_proposal.Deadline.ToString(),
											_proposal.Accepted.ToString());
		}

		// -------------------------------------------
		/* 
		 * UpdateProposal
		 */
		public static void UpdateProposal(string _idUser, string _password, ProposalModel _proposal)
		{
			CommController.Instance.Request(EVENT_COMM_UPDATE_PROPOSAL, true, _idUser, _password,
											_proposal.Id.ToString(),
											_proposal.Request.ToString(),
											_proposal.Price.ToString(),
											_proposal.Deadline.ToString(),
											_proposal.User.ToString(),
											_proposal.Accepted.ToString());
		}

		// -------------------------------------------
		/* 
		 * RemoveProposal
		 */
		public static void RemoveProposal(string _idUser, string _password, long _idProposal)
		{
			CommController.Instance.Request(EVENT_COMM_REMOVE_PROPOSAL, true, _idUser, _password, _idProposal.ToString());
		}


		// -------------------------------------------
		/* 
		 * ConsultAllProposalsByRequest
		 */
		public static void ConsultAllProposalsByRequest(int _id, string _password, long _idRequest)
		{
			CommController.Instance.Request(EVENT_COMM_CONSULT_ALL_PROPOSALS_OF_REQUEST, false, _id.ToString(), _password, _idRequest.ToString());
		}

		// -------------------------------------------
		/* 
		 * ResetProposalsForRequest
		 */
		public static void ResetProposalsForRequest(int _id, string _password, long _idRequest)
		{
			CommController.Instance.Request(EVENT_COMM_RESET_ALL_PROPOSALS_OF_REQUEST, true, _id.ToString(), _password, _idRequest.ToString());
		}

		// -------------------------------------------
		/* 
		 * ReactivateProposal
		 */
		public static void ReactivateProposal(int _id, string _password, long _idProposal)
		{
			CommController.Instance.Request(EVENT_COMM_REACTIVATE_PROPOSAL, true, _id.ToString(), _password, _idProposal.ToString());
		}

		// -------------------------------------------
		/* 
		 * ReportProposal
		 */
		public static void ReportProposal(int _id, string _password, long _idProposal, int _idReporter, long _idRequest)
		{
			CommController.Instance.Request(EVENT_COMM_REPORT_PROPOSAL, true, _id.ToString(), _password, _idProposal.ToString(), _idReporter.ToString(), _idRequest.ToString());
		}

		// -------------------------------------------
		/* 
		 * LoadImage
		 */
		public static void LoadImage(long _idImage)
		{
			CommController.Instance.Request(EVENT_COMM_LOAD_IMAGE, true, _idImage.ToString());
		}

		// -------------------------------------------
		/* 
		 * RentTimeAsAProvider
		 */
		public static void IAPRegisterCode(int _id, string _password, string _codeRegister)
		{
			CommController.Instance.Request(EVENT_COMM_IAP_CODE_REGISTER, true, _id.ToString(), _password, _codeRegister);
		}

		// -------------------------------------------
		/* 
		 * RentTimeAsAProvider
		 */
		public static void IAPRentTimeAsAProvider(int _id, string _password, int _rentValue, string _codeValidation)
		{
			CommController.Instance.Request(EVENT_COMM_IAP_RENT_TIME_PROVIDER, true, _id.ToString(), _password, _rentValue.ToString(), _codeValidation);
		}

		// -------------------------------------------
		/* 
		 * RentTimeAsAProvider
		 */
		public static void IAPPurchasePremiumOffer(int _id, string _password, string _codeValidation)
		{
			CommController.Instance.Request(EVENT_COMM_IAP_PREMIUM_OFFER, true, _id.ToString(), _password, _codeValidation);
		}

		// -------------------------------------------
		/* 
		 * IAPPurchasePremiumRequest
		 */
		public static void IAPPurchasePremiumRequest(int _id, string _password, string _codeValidation)
		{
			CommController.Instance.Request(EVENT_COMM_IAP_PREMIUM_REQUEST, true, _id.ToString(), _password, _codeValidation);
		}

		// -------------------------------------------
		/* 
		 * RequestUpdateTransactionBitcoin
		 */
		public static void RequestUpdateTransactionBlockchain(int _idUser, string _password, long _requestId, string _transactionID)
		{
			CommController.Instance.Request(EVENT_COMM_BITCOIN_REGISTER_TRANSACTION, true, _idUser.ToString(), _password, _requestId.ToString(), _transactionID);
		}

	}
}
