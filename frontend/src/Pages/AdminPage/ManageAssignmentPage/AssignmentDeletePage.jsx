import { Button, Divider, Modal, Space } from "antd";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { checkCanDisableUser, disableUser } from "../../../Apis/UserApis";

export function AssignmentDeletePage() {
  let { id } = useParams();
  const navigate = useNavigate();
  const [isModalOpen, setIsModalOpen] = useState(true);
  const [loadings, setLoadings] = useState([]);
  const enterLoading = (index) => {
    setLoadings((prevLoadings) => {
      const newLoadings = [...prevLoadings];
      newLoadings[index] = true;
      return newLoadings;
    });
    setTimeout(() => {
      setLoadings((prevLoadings) => {
        const newLoadings = [...prevLoadings];
        newLoadings[index] = false;
        return newLoadings;
      });
    }, 6000);
  };

  const handleDelete = async () => {
    enterLoading();
    setIsModalOpen(false);
    navigate(-1);
  };

  const handleOnclick = () => {
    setIsModalOpen(false);
    navigate(-1);
  };

  return (
    <>
      <Modal open={isModalOpen} closable={false} footer={false} className="w-fit">
        <div className="flex content-center justify-between">
          <h1 className="pl-5 text-2xl font-bold text-red-600">
            Are you sure?
          </h1>
        </div>
        <Divider />
        <div className="pl-5 pb-5">
          <p className="mb-5 text-base">Do you want to delete this assignment?</p>
          <Space className="mt-5">
            <Button
              type="primary"
              danger
              className="mr-2"
              loading={loadings[0]}
              onClick={handleDelete}
            >
              Delete
            </Button>
            <Button onClick={handleOnclick}>Cancel</Button>
          </Space>
        </div>
      </Modal>
    </>
  );
}
