import axios, { AxiosResponse } from "axios";
import { ISession, ISessionsEnvelope } from "../models/session";
import { history } from "../..";
import { toast } from "react-toastify";
import { IUser, IUserFormValues } from "../models/user";
import { IProfile, IPhoto } from "../models/profile";
import { IKid } from "../models/kid";
import { request } from "https";

axios.defaults.baseURL = "http://localhost:5000/api";

axios.interceptors.request.use(
  (config) => {
    const token = window.localStorage.getItem("jwt");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

axios.interceptors.response.use(undefined, (error) => {
  if (error.message === "NetworkError" && !error.response) {
    toast.error("Network error - make sure API is running!");
  }
  const { status, data, config } = error.response;
  if (error.response.status === 404) {
    history.push("/notfound");
  }
  if (
    error.response.status === 400 &&
    config.method === "get" &&
    data.errors.hasOwnProperty("id")
  ) {
    history.push("/notfound");
  }
  if (status === 500) {
    toast.error("Server error - check terminal for more info!");
  }
  throw error.response;
});

const responseBody = (response: AxiosResponse) => response.data;

const sleep = (ms: number) => (response: AxiosResponse) =>
  new Promise<AxiosResponse>((resolve) =>
    setTimeout(() => resolve(response), ms)
  );

const requests = {
  get: (url: string) =>
    axios
      .get(url)
      .then(sleep(1000))
      .then(responseBody),
  post: (url: string, body: {}) =>
    axios
      .post(url, body)
      .then(sleep(1000))
      .then(responseBody),
  put: (url: string, body: {}) =>
    axios
      .put(url, body)
      .then(sleep(1000))
      .then(responseBody),
  del: (url: string) =>
    axios
      .delete(url)
      .then(sleep(1000))
      .then(responseBody),
  postForm: (url: string, file: Blob) => {
    let formData = new FormData();
    formData.append("File", file);
    return axios
      .post(url, formData, {
        headers: { "Content-type": "multipart/formdata" }
      })
      .then(responseBody);
  }
};

const Sessions = {
  list: (params: URLSearchParams): Promise<ISessionsEnvelope> =>
    axios
      .get("/sessions", { params: params })
      .then(sleep(1000))
      .then(responseBody),
  details: (id: string) => requests.get(`/sessions/${id}`),
  create: (session: ISession) => requests.post("/sessions", session),
  update: (session: ISession) =>
    requests.put(`/sessions/${session.id}`, session),
  delete: (id: string) => requests.del(`/sessions/${id}`),
  attend: (id: string) => requests.post(`/sessions/${id}/attend`, {}),
  unattend: (id: string) => requests.del(`/sessions/${id}/attend`)
};

const User = {
  current: (): Promise<IUser> => requests.get("/user"),
  login: (user: IUserFormValues): Promise<IUser> =>
    requests.post(`/user/login`, user),
  register: (user: IUserFormValues): Promise<IUser> =>
    requests.post(`/user/register`, user)
};

const Profiles = {
  get: (username: string): Promise<IProfile> =>
    requests.get(`/profiles/${username}`),
  uploadPhoto: (photo: Blob): Promise<IPhoto> =>
    requests.postForm(`/photos`, photo),
  setMainPhoto: (id: string) => requests.post(`/photos/${id}/setMain`, {}),
  deletePhoto: (id: string) => requests.del(`/photos/${id}`),
  updateProfile: (profile: Partial<IProfile>) =>
    requests.put(`/profiles`, profile)
};

const Kids = {
  getKid: (username: string): Promise<IKid> =>
    requests.get(`/${username}/kids`),
  addKid: (username: string): Promise<IKid> => requests.get(`/${username}/kids`)
};
export default {
  Sessions,
  User,
  Profiles,
  Kids
};
