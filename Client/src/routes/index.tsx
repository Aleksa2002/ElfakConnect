import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
  component: Index,
});

function Index() {
  const handleLoginWithMicrosoft = () => {
    window.location.href =
      "https://localhost:7267/api/account/login/microsoft?returnUrl=http://localhost:5173/";
  };

  return (
    <>
      <div>
        <button onClick={handleLoginWithMicrosoft}>
          Sign in with Microsoft
        </button>
      </div>
    </>
  );
}
