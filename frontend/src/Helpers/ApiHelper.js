import axios from "axios";
import { TOKEN_KEY } from "../Constants/SystemConstants";

export async function callApi(method, url, data = null) {
  let response = undefined;

  await axios({
    method: method,
    url: url,
    headers: { Authorization: localStorage.getItem(TOKEN_KEY) },
    data: data,
  })
    .then((result) => {
      response = result.data;
    })
    .catch((error) => {
      throw new Response("", {
        status: error.response.status,
        statusText: error.message,
      });
    });

  return response;
}
