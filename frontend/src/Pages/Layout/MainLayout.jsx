import { Link, Outlet, useLocation } from "react-router-dom";
import { Layout, Menu } from "antd";
import React from "react";
import "./MainLayout.css";
import nashLogo from "../../Assets/nashLogo.jpg";
import { DropdownLayout } from "./DropdownLayout";

export function MainLayout() {
  const location = useLocation();
  const { Header, Content, Footer, Sider } = Layout;

  const adminPages = [
    { name: "Manage User", path: "/admin/manage-user" },
    { name: "Manage Asset", path: "/admin/manage-asset" },
    { name: "Manage Assignment", path: "/admin/manage-assignment" },
    {
      name: "Manage Returning",
      path: "/admin/manage-returning",
    },
    { name: "Report", path: "/admin/report" },
  ];

  const staffPages = [];

  return (
    <div>
      <Layout>
        <Header
          className="header"
          style={{
            padding: 0,
            backgroundColor: "red",
          }}
        >
          <DropdownLayout />
        </Header>
        <Layout className="LayoutContent">
          <Sider
            className="siderLayout"
            breakpoint="lg"
            collapsedWidth="0"
            theme="light"
            onBreakpoint={(broken) => {
              console.log(broken);
            }}
            onCollapse={(collapsed, type) => {
              console.log(collapsed, type);
            }}
          >
            <div className="divLogo">
              <img alt="logoNashTech" src={nashLogo} className="logo"></img>
            </div>
            <div>
              <h4 className="title"> Online Asset Management</h4>
            </div>

            <Menu
              className="menuSider"
              theme="light"
              mode="inline"
              selectedKeys={location.pathname}
            >
              <Menu.Item className="menuItem" key="/">
                <Link to="/">Home</Link>
              </Menu.Item>

              {adminPages.map((page) => (
                <Menu.Item className="menuItem" key={page.path}>
                  <Link to={page.path}>{page.name}</Link>
                </Menu.Item>
              ))}

              {staffPages.map((page) => (
                <Menu.Item className="menuItem" key={page.path}>
                  <Link to={page.path}>{page.name}</Link>
                </Menu.Item>
              ))}
            </Menu>
          </Sider>

          <Content
            style={{
              margin: "24px 16px 0",
            }}
          >
            <div
              className="site-layout-background"
              style={{
                padding: 24,
                minHeight: 600,
              }}
            >
              <Outlet />
            </div>
          </Content>
        </Layout>

        <Footer
          className="footerLayout"
          style={{
            backgroundColor: "red",
            color: "white",
          }}
        >
          NashTech2022 Part of Nash Squared.
        </Footer>
      </Layout>
    </div>
  );
}
