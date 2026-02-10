enum RoleEnum {
  TEACHER = 'TEACHER',
  INSTITUTION = 'INSTITUTION',
  STUDENT = 'STUDENT',
  //other roles tbd...
}

/**
 * POST
 * https://identity-url/endpoint/neispuo-user
 */
interface PostRequest {
  firstName: string;
  middleName: string;
  lastName: string;
  fullName: string;
  neispuoID: number;
  roles: [RoleEnum];
  createdBy: string; //module or user identifier
}

interface PostResponse {
  error?: string;

}

/**
 * PUT
 * https://identity-url/endpoint/neispuo-user/identity-user-uuid
 */
interface PutRequest extends PostRequest {
  error?: string;
}


interface PutResponse extends PostRequest {
  error?: string;
}


/**]
 * DELETE
 * https://identity-url/endpoint/neispuo-user/identity-user-uuid
 */

