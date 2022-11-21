import { Route, Routes, useLocation } from "react-router-dom";
import { AuthState } from "./Contexts/AuthState";
import {
  AdminPage,
  ChangePasswordPage,
  HomePage,
  LoginPage,
  LogoutPage,
  MainLayout,
  ManageAssetPage,
  ManageAssignmentPage,
  ManageRequestForReturningPage,
  ManageUserPage,
  ReportPage,
} from "./Pages";

function App() {
  const location = useLocation();
  const background = location.state && location.state.background;

  const mainRoutes = (
    <Routes location={background || location}>
      <Route path="/" element={<MainLayout />}>
        <Route index={true} element={<HomePage />} />
        <Route path="/admin" element={<AdminPage />}>
          <Route path="/admin/manage-asset" element={<ManageAssetPage />} />
          <Route path="/admin/manage-user" element={<ManageUserPage />} />
          <Route
            path="/admin/manage-assignment"
            element={<ManageAssignmentPage />}
          />
          <Route
            path="/admin/manage-returning"
            element={<ManageRequestForReturningPage />}
          />
          <Route path="/admin/report" element={<ReportPage />} />
        </Route>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/change-password" element={<ChangePasswordPage />} />
        <Route path="/logout" element={<LogoutPage />} />
      </Route>
    </Routes>
  );

  const modalRoutes = (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/change-password" element={<ChangePasswordPage />} />
      <Route path="/logout" element={<LogoutPage />} />
    </Routes>
  );

  return (
    <AuthState>
      {mainRoutes}
      {background && modalRoutes}
    </AuthState>
  );
}

export default App;
