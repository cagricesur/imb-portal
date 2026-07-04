import { Outlet } from "@tanstack/react-router";

const Master: React.FunctionComponent = () => {
  return (
    <div>
      <span>Master</span>
      <div>
        <Outlet />
      </div>
    </div>
  );
};

export default Master;
