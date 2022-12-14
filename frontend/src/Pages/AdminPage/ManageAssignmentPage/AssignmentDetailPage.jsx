import { Divider, Modal } from "antd";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { getAssignmentById } from "../../../Apis/AssignmentApis";

export function AssignmentDetailPage() {
  const [data, setData] = useState({});
  const { assignmentId } = useParams();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const loadData = async () => {
      const res = await getAssignmentById(assignmentId);

      setData(res);
      setIsModalOpen(true);
    };

    loadData();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const onCancel = () => {
    navigate(-1);
  };

  return (
    <>
      <Modal
        open={isModalOpen}
        closable={true}
        footer={false}
        onCancel={onCancel}
      >
        <div className="flex items-center justify-between">
          <h1 className="ml-7 -mt-2 text-2xl font-bold text-red-600">
            Detailed Assignment Information
          </h1>
        </div>
        <Divider className="mb-0" />
        <div>
          <table className="ml-5 -mb-3 border-separate border-spacing-3">
            <tbody>
              <tr>
                <td className="font-bold">Asset Code:</td>
                <td>{data.assetCode}</td>
              </tr>
              <tr>
                <td className="font-bold">Asset Name:</td>
                <td className="break-all">{data.assetName}</td>
              </tr>
              <tr>
                <td className="font-bold">Specification:</td>
                <td className="break-all">{data.specification}</td>
              </tr>
              <tr>
                <td className="font-bold">Assigned to:</td>
                <td className="break-all">{data.assignedTo}</td>
              </tr>
              <tr>
                <td className="font-bold">Assigned by:</td>
                <td className="break-all">{data.assignedBy}</td>
              </tr>
              <tr>
                <td className="font-bold">Assigned date:</td>
                <td>{data.assignedDate}</td>
              </tr>
              <tr>
                <td className="font-bold">State:</td>
                <td>{data.state}</td>
              </tr>
              <tr>
                <td className="font-bold">Note:</td>
                <td className="break-all">{data.note}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </Modal>
    </>
  );
}
