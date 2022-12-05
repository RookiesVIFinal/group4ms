import { callApi } from "../Helpers/ApiHelper";
import { API_BASE_URL } from "../Constants/SystemConstants";

const url = `${API_BASE_URL}/api/categories`;

export async function getAllCategories() {
  return await callApi("get", url + "/all");
}