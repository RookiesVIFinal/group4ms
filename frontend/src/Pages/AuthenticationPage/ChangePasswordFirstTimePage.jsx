import { Button, Form, Input, Modal } from "antd";
import { useNavigate } from "react-router-dom";
import { changePassword } from "../../Apis/Accounts";
import { TOKEN_KEY } from "../../Constants/SystemConstants";
import {
  PASSWORD_REQUIRED,
  PASSWORD_AT_LEAST_ONE_DIGIT,
  PASSWORD_AT_LEAST_ONE_SPECIAL_CHARACTER,
  PASSWORD_AT_LEAST_ONE_LOWERCASE,
  PASSWORD_AT_LEAST_ONE_UPPERCASE,
  PASSWORD_RANGE_FROM_8_TO_16_CHARACTERS,
} from "../../Constants/ErrorMessages";

export function ChangePasswordFirstTimePage() {
  const navigate = useNavigate();

  const onFinish = async (values) => {
    console.log(values);
    console.log(localStorage.getItem(TOKEN_KEY));

    await changePassword({ ...values });

    navigate("/");
  };

  const onFinishFailed = (errorInfo) => {
    console.log("Failed:", errorInfo);
  };

  return (
    <Modal title="Change Password" open={true} closable={false} footer={false}>
      <p>
        This is the first time you logged in.
        <br />
        You have to change your password to continue.
      </p>
      <Form
        name="basic"
        labelCol={{
          span: 8,
        }}
        wrapperCol={{
          span: 16,
        }}
        initialValues={{
          remember: true,
        }}
        onFinish={onFinish}
        onFinishFailed={onFinishFailed}
        autoComplete="off"
      >
        <Form.Item
          label="New Password"
          name="newPassword"
          rules={[
            {
              required: true,
              message: PASSWORD_REQUIRED,
            },
            {
              pattern: /^(?=.*[0-9])[A-Za-z0-9!*_@#$%^&+= ]*$/,
              message: PASSWORD_AT_LEAST_ONE_DIGIT,
            },
            {
              pattern: /^(?=.*[a-z])[A-Za-z0-9!*_@#$%^&+= ]*$/,
              message: PASSWORD_AT_LEAST_ONE_LOWERCASE,
            },
            {
              pattern: /^(?=.*[A-Z])[A-Za-z0-9!*_@#$%^&+= ]*$/,
              message: PASSWORD_AT_LEAST_ONE_UPPERCASE,
            },
            {
              pattern: /^(?=.*[!*_@#$%^&+= ]).*$/,
              message: PASSWORD_AT_LEAST_ONE_SPECIAL_CHARACTER,
            },
            {
              min: 8,
              max: 16,
              message: PASSWORD_RANGE_FROM_8_TO_16_CHARACTERS,
            },
          ]}
        >
          <Input.Password />
        </Form.Item>
        <Form.Item
          wrapperCol={{
            offset: 8,
            span: 16,
          }}
        >
          <Button type="primary" htmlType="submit" danger>
            Save
          </Button>
        </Form.Item>
      </Form>
    </Modal>
  );
}